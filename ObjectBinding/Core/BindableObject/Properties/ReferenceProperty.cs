using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject.Properties
{
  public class ReferenceProperty : PropertyBase, IBusinessObjectReferenceProperty
  {
    private readonly Type _concreteType;
    private readonly DoubleCheckedLockingContainer<IBusinessObjectClass> _referenceClass;
    private readonly Type _searchServiceType;

    public ReferenceProperty (Parameters parameters, Type concreteType)
        : base (parameters)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, UnderlyingType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, typeof (IBusinessObject));

      _concreteType = concreteType;
      _referenceClass = new DoubleCheckedLockingContainer<IBusinessObjectClass> (delegate { return GetReferenceClass(); });
      _searchServiceType = GetSearchServiceType();
    }

    /// <summary> Gets the class information for elements of this property. </summary>
    /// <value>The <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> accessed through this property.</value>
    public IBusinessObjectClass ReferenceClass
    {
      get { return _referenceClass.Value; }
    }

    /// <summary> 
    ///   Gets a flag indicating whether it is possible to get a list of the objects that can be assigned to this property.
    /// </summary>
    /// <param name="supportsIdentity">
    /// A flag that if <see langword="true"/> determines that the objects returned by <see cref="SearchAvailableObjects"/> will implement 
    /// <see cref="IBusinessObjectWithIdentity"/>.
    /// </param>
    /// <returns> <see langword="true"/> if it is possible to get the available objects from the object model. </returns>
    /// <remarks> 
    ///   Use the <see cref="SearchAvailableObjects"/> method to get the list of objects.
    /// </remarks>
    public bool SupportsSearchAvailableObjects (bool supportsIdentity)
    {
      ISearchAvailableObjectsService searchAvailableObjectsService = (ISearchAvailableObjectsService) BusinessObjectProvider.GetService (_searchServiceType);
      if (searchAvailableObjectsService == null)
        return false;

      if (supportsIdentity)
        return searchAvailableObjectsService.SupportsIdentity (this);

      return true;
    }

    /// <summary> 
    ///   Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.
    /// </summary>
    /// <param name="requiresIdentity">
    /// A flag that if <see langword="true"/> determines that the return value muss consist only of objects implenting 
    /// <see cref="IBusinessObjectWithIdentity"/>.
    /// </param>
    /// <param name="referencingObject"> The business object for which to search for the possible objects to be referenced. </param>
    /// <param name="requiresIdentity">
    /// A flag that if <see langword="true"/> determines that the return value muss consist only of objects implenting 
    /// <see cref="IBusinessObjectWithIdentity"/>.
    /// </param>
    /// <param name="searchStatement"> 
    ///   A <see cref="string"/> containing a search statement. Can be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///   Thrown if <see cref="SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method has been called anyways.
    /// </exception>
    public IBusinessObject[] SearchAvailableObjects (IBusinessObject referencingObject, bool requiresIdentity, string searchStatement)
    {
      ArgumentUtility.CheckNotNull ("referencingObject", referencingObject);

      if (!SupportsSearchAvailableObjects (requiresIdentity))
      {
        throw new NotSupportedException (
            string.Format (
                "Searching is not supported for reference property '{0}' of business object class '{1}'.",
                Identifier,
                referencingObject.BusinessObjectClass.Identifier));
      }

      ISearchAvailableObjectsService searchAvailableObjectsService = (ISearchAvailableObjectsService) BusinessObjectProvider.GetService (_searchServiceType);
      Assertion.Assert (searchAvailableObjectsService != null, "The BusinessObjectProvider did not return a service for '{0}'.", _searchServiceType.FullName);

      return searchAvailableObjectsService.Search (referencingObject, this, searchStatement);
    }

    /// <summary>
    ///   Gets a flag indicating if <see cref="Create"/> may be called to implicitly create a new business object 
    ///   for editing in case the object reference is null.
    /// </summary>
    public bool CreateIfNull
    {
      get { return false; }
    }

    /// <summary>
    ///   If <see cref="CreateIfNull"/> is <see langword="true"/>, this method can be used to create a new business 
    ///   object.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The business object containing the reference property whose value will be assigned the newly created object. 
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="CreateIfNull"/> evaluated <see langword="false"/>. 
    /// </exception>
    public IBusinessObject Create (IBusinessObject referencingObject)
    {
      throw new NotSupportedException (string.Format ("Create method is not supported by '{0}'.", GetType().FullName));
    }

    private IBusinessObjectClass GetReferenceClass ()
    {
      if (IsBindableObjectImplementation())
        return BusinessObjectProvider.GetBindableObjectClass (UnderlyingType);

      return GetReferenceClassFromService();
    }

    private bool IsBindableObjectImplementation ()
    {
      return AttributeUtility.IsDefined<IBindableObjectAttribute> (_concreteType, true);
    }

    private IBusinessObjectClass GetReferenceClassFromService ()
    {
      IBusinessObjectClassService service = GetBusinessObjectClassService();
      IBusinessObjectClass businessObjectClass = service.GetBusinessObjectClass (UnderlyingType);
      if (businessObjectClass == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The GetBusinessObjectClass method of '{0}', registered with the '{1}', failed to return an '{2}' for type '{3}'.",
                service.GetType().FullName,
                BusinessObjectProvider.GetType().FullName,
                typeof (IBusinessObjectClass).FullName,
                UnderlyingType.FullName));
      }

      return businessObjectClass;
    }

    private IBusinessObjectClassService GetBusinessObjectClassService ()
    {
      IBusinessObjectClassService service = BusinessObjectProvider.GetService<IBusinessObjectClassService>();
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' type does not use the '{1}' implementation of '{2}' and there is no '{3}' registered with the '{4}'.",
                UnderlyingType.FullName,
                typeof (BindableObjectMixin).Namespace,
                typeof (IBusinessObject).FullName,
                typeof (IBusinessObjectClassService).FullName,
                BusinessObjectProvider.GetType().FullName));
      }
      return service;
    }

    private Type GetSearchServiceType ()
    {
      SearchAvailableObjectsServiceTypeAttribute attribute;
      attribute = AttributeUtility.GetCustomAttribute<SearchAvailableObjectsServiceTypeAttribute> (PropertyInfo, true);
      if (attribute == null)
        attribute = AttributeUtility.GetCustomAttribute<SearchAvailableObjectsServiceTypeAttribute> (_concreteType, true);
      if (attribute == null)
        return typeof (ISearchAvailableObjectsService);
      return attribute.Type;
    }
  }
}