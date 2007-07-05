using System;
using Rubicon.Mixins;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class ReferenceProperty : PropertyBase, IBusinessObjectReferenceProperty
  {
    private readonly Type _concreteType;
    private readonly DoubleCheckedLockingContainer<IBusinessObjectClass> _referenceClass;

    public ReferenceProperty (Parameters parameters, Type concreteType)
        : base (parameters)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, typeof (IBusinessObject));

      _concreteType = concreteType;
      _referenceClass = new DoubleCheckedLockingContainer<IBusinessObjectClass> (delegate { return GetReferenceClass(); });
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
    /// <param name="requiresIdentity">
    /// A flag that if <see langword="true"/> determines that the objects must implement <see cref="IBusinessObjectWithIdentity"/>.
    /// </param>
    /// <returns> <see langword="true"/> if it is possible to get the available objects from the object model. </returns>
    /// <remarks> 
    ///   Use the <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> method (or an object model specific overload) to get the list of 
    ///   objects.
    /// </remarks>
    public bool SupportsSearchAvailableObjects (bool requiresIdentity)
    {
      //Type searchServiceType = AttributeUtility.GetCustomAttribute<BindableObjectSearchServiceTypeAttribte> (ReferenceClass.Type, true).Type;
      //IBusinessObjectSearchService searchService = BusinessObjectProvider.GetService (searchServiceType);
      //if (searchService == null)
      //  return requiresIdentity ? new IBusinessObjectWithIdentity[0] : new IBusinessObject[0];

      //if (requiresIdentity && !searchService.SupportsIdentity (this, obj))
      //  throw new NotSupportedException ();

      throw new NotImplementedException();
    }

    /// <summary> 
    ///   Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.
    /// </summary>
    /// <param name="requiresIdentity">
    /// A flag that if <see langword="true"/> determines that the return value muss consist only of objects implenting 
    /// <see cref="IBusinessObjectWithIdentity"/>.
    /// <param name="obj"> The business object for which to search for the possible objects to be referenced. </param>
    /// <param name="searchStatement"> 
    ///   A <see cref="string"/> containing a search statement. Can be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///   Thrown if <see cref="IBusinessObjectReferenceProperty.SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method
    ///   has been called anyways.
    /// </exception>
    /// <remarks> 
    ///   This method is used if the seach statement is entered via the Visual Studio .NET designer, for instance in
    ///   the <see cref="T:Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue"/> control.
    ///   <note type="inotes">
    ///     If your object model cannot evaluate a search string, but allows search through a less generic method,
    ///     provide an overload, and document that getting the list of available objects is only possible during runtime.
    ///   </note>
    /// </remarks>
    public IBusinessObject[] SearchAvailableObjects (bool requiresIdentity, IBusinessObject obj, string searchStatement)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      //Type searchServiceType = AttributeUtility.GetCustomAttribute<BusinessObjectSearchServiceTypeAttribte> (ReferenceClass.Type, true).Type;
      //IBusinessObjectSearchService searchService = BusinessObjectProvider.GetService (searchServiceType);
      //if (searchService == null)
      //  return requiresIdentity ? new IBusinessObjectWithIdentity[0] : new IBusinessObject[0];

      //if (requiresIdentity && !searchService.SupportsIdentity (this, obj))
      //  throw new NotSupportedException ();

      //return searchService.Search (this, obj, searchStatement);
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Gets a flag indicating if <see cref="IBusinessObjectReferenceProperty.Create"/> may be called to implicitly create a new business object 
    ///   for editing in case the object reference is null.
    /// </summary>
    public bool CreateIfNull
    {
      get { return false; }
    }

    /// <summary>
    ///   If <see cref="IBusinessObjectReferenceProperty.CreateIfNull"/> is <see langword="true"/>, this method can be used to create a new business 
    ///   object.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The business object containing the reference property whose value will be assigned the newly created object. 
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="IBusinessObjectReferenceProperty.CreateIfNull"/> evaluated <see langword="false"/>. 
    /// </exception>
    /// <remarks>
    ///   A use case for the <b>Create</b> method is the instantiation of an business object without a unique identifier,
    ///   usually an <b>Aggregate</b>. The aggregate reference can be <see langword="null"/> until one of its values
    ///   is set in the user interface.
    /// </remarks>
    public IBusinessObject Create (IBusinessObject referencingObject)
    {
      throw new NotSupportedException (string.Format ("Create method is not supported by '{0}'.", GetType().FullName));
    }

    private IBusinessObjectClass GetReferenceClass ()
    {
      if (AttributeUtility.IsDefined<IBindableObjectAttribute>(_concreteType, true))
        return BusinessObjectProvider.GetBindableObjectClass (UnderlyingType);

      return GetReferenceClassFromService();
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
  }
}