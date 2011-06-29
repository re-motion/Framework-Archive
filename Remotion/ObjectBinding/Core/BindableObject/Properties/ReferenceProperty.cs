// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class ReferenceProperty : PropertyBase, IBusinessObjectReferenceProperty
  {
    private enum ServiceProvider
    {
      PropertyType,
      DeclaringType
    }

    private readonly Type _concreteType;
    private readonly DoubleCheckedLockingContainer<IBusinessObjectClass> _referenceClass;
    private readonly Tuple<ServiceProvider, Type> _searchServiceDefinition;
    private readonly Tuple<ServiceProvider, Type> _createObjectServiceDefinition;

    public ReferenceProperty (Parameters parameters)
        : base (parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckTypeIsAssignableFrom ("parameters.ConcreteType", parameters.ConcreteType, typeof (IBusinessObject));

      _concreteType = parameters.ConcreteType;
      _referenceClass = new DoubleCheckedLockingContainer<IBusinessObjectClass> (GetReferenceClass);
      _searchServiceDefinition = GetServiceDeclaration<SearchAvailableObjectsServiceTypeAttribute, ISearchAvailableObjectsService>();
      _createObjectServiceDefinition = GetServiceDeclaration<CreateObjectServiceTypeAttribute, ICreateObjectService>();
    }

    /// <summary> Gets the class information for elements of this property. </summary>
    /// <value>The <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> accessed through this property.</value>
    public IBusinessObjectClass ReferenceClass
    {
      get { return _referenceClass.Value; }
    }

    /// <summary>Gets a flag indicating whether it is possible to get a list of the objects that can be assigned to this property.</summary>
    /// <returns> <see langword="true"/> if it is possible to get the available objects from the object model. </returns>
    /// <remarks>
    /// <para>Use the <see cref="SearchAvailableObjects"/> method to get the list of objects.</para>
    /// <para>If the <see cref="ReferenceClass"/> implements <see cref="IBusinessObjectClassWithIdentity"/>, 
    /// the <see cref="ISearchAvailableObjectsService.SupportsProperty"/> method of the <see cref="ISearchAvailableObjectsService"/> interface 
    /// is evaluated in order to determine the return value of this property.
    /// </para>
    /// </remarks>
    public bool SupportsSearchAvailableObjects
    {
      get
      {
        ISearchAvailableObjectsService searchService = GetSearchService();
        if (searchService == null)
          return false;

        return searchService.SupportsProperty (this);
      }
    }

    /// <summary>Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.</summary>
    /// <param name="referencingObject"> The business object for which to search for the possible objects to be referenced. </param>
    /// <param name="searchArguments">A parameter-object containing additional information for executing the search. Can be <see langword="null"/>.</param>
    /// <returns>A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.</returns>
    /// <exception cref="NotSupportedException">
    ///   Thrown if <see cref="SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method has been called anyways.
    /// </exception>
    public IBusinessObject[] SearchAvailableObjects (IBusinessObject referencingObject, ISearchAvailableObjectsArguments searchArguments)
    {
      if (!SupportsSearchAvailableObjects)
      {
        throw new NotSupportedException (
            string.Format (
                "Searching is not supported for reference property '{0}' of business object class '{1}'.",
                Identifier,
                ReflectedClass.Identifier));
      }

      ISearchAvailableObjectsService searchService = GetSearchService ();
      Assertion.IsNotNull (searchService, "The BusinessObjectProvider did not return a service for '{0}'.", _searchServiceDefinition.Item2.FullName);

      return searchService.Search (referencingObject, this, searchArguments);
    }

    /// <summary>
    ///   Gets a flag indicating if <see cref="Create"/> may be called to implicitly create a new business object 
    ///   for editing in case the object reference is null.
    /// </summary>
    public bool CreateIfNull
    {
      get
      {
        ICreateObjectService createObjectService = GetCreateObjectService ();
        if (createObjectService == null)
          return false;

        return createObjectService.SupportsProperty (this);
      }
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
      if (BindableObjectProvider.IsBindableObjectImplementation (UnderlyingType))
      {
        var provider = BindableObjectProvider.GetProviderForBindableObjectType (UnderlyingType);
        return provider.GetBindableObjectClass (UnderlyingType);
      }

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
                "The '{0}' type does not use the '{1}' implementation of '{2}' and there is no '{3}' registered with the '{4}' associated with this type.",
                UnderlyingType.FullName,
                typeof (BindableObjectMixin).Namespace,
                typeof (IBusinessObject).FullName,
                typeof (IBusinessObjectClassService).FullName,
                typeof (BusinessObjectProvider).FullName));
      }
      return service;
    }

    private ISearchAvailableObjectsService GetSearchService ()
    {
      return GetService < ISearchAvailableObjectsService> (_searchServiceDefinition);
    }

    private ICreateObjectService GetCreateObjectService ()
    {
      return GetService<ICreateObjectService> (_createObjectServiceDefinition);
    }

    private TService GetService<TService> (Tuple<ServiceProvider, Type> serviceDefinition)
        where TService : IBusinessObjectService
    {
      IBusinessObjectProvider provider;
      switch (serviceDefinition.Item1)
      {
        case ServiceProvider.DeclaringType:
          provider = BusinessObjectProvider;
          break;
        case ServiceProvider.PropertyType:
          provider = ReferenceClass.BusinessObjectProvider;
          break;
        default:
          throw new InvalidOperationException();
      }

      return (TService) provider.GetService (serviceDefinition.Item2);
    }

    private Tuple<ServiceProvider, Type> GetServiceDeclaration<TServiceTypeAttribute, TService> () 
        where TServiceTypeAttribute : Attribute, IBusinessObjectServiceTypeAttribute
        where TService : IBusinessObjectService
    {
      var attributeFromDeclaringType = PropertyInfo.GetCustomAttribute<TServiceTypeAttribute> (true);
      if (attributeFromDeclaringType != null)
        return new Tuple<ServiceProvider, Type> (ServiceProvider.DeclaringType, attributeFromDeclaringType.Type);

      var attributeFromPropertyType = AttributeUtility.GetCustomAttribute<TServiceTypeAttribute> (_concreteType, true);
      if (attributeFromPropertyType != null)
        return new Tuple<ServiceProvider, Type> (ServiceProvider.PropertyType, attributeFromPropertyType.Type);

      return new Tuple<ServiceProvider, Type> (ServiceProvider.DeclaringType, typeof (TService));
    }
  }
}
