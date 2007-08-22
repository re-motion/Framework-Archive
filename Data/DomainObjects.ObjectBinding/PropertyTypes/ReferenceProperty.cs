using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  public class ReferenceProperty: NullableProperty, IBusinessObjectReferenceProperty
  {
    public ReferenceProperty (
        IBusinessObjectClass businessObjectClass,
        PropertyInfo propertyInfo,
        bool isRequired,
        Type itemType,
        bool isList)
      : base (businessObjectClass, propertyInfo, isRequired, itemType, isList, true)
    {
    }

    public IBusinessObjectClass ReferenceClass
    {
      get
      {
        DomainObjectClass domainObjectClass = BusinessObjectClass as DomainObjectClass;
        if (domainObjectClass != null && domainObjectClass.ClassDefinition is ReflectionBasedClassDefinition)
        {
          if (typeof (BindableDomainObject).IsAssignableFrom (UnderlyingType))
            return new DomainObjectClass (UnderlyingType);

          return GetReferenceClassFromService ();
        }
        else
        {
          return new DomainObjectClass ((IsList) ? ListInfo.ItemType : PropertyType);
        }
      }
    }

    public IBusinessObject[] SearchAvailableObjects (IBusinessObject businessObject, bool requiresIdentity, string queryID)
    {
      if (queryID == null || queryID == string.Empty)
        return new IBusinessObjectWithIdentity[] { };

      QueryDefinition definition = QueryConfiguration.Current.QueryDefinitions.GetMandatory (queryID);
      if (definition.QueryType != QueryType.Collection)
        throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", queryID), "queryID");

      ClientTransaction clientTransaction = ClientTransactionScope.CurrentTransaction;

      DomainObjectCollection result = clientTransaction.QueryManager.GetCollection (new Query (definition));
      IBusinessObjectWithIdentity[] availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.CopyTo (availableObjects, 0);

      return availableObjects;
    }

    public bool SupportsSearchAvailableObjects (bool supportsIdentity)
    {
      return true;
    }

    public bool CreateIfNull
    {
      get { return false; }
    }

    public IBusinessObject Create (IBusinessObject referencingObject)
    {
      throw new NotSupportedException ("Create method is not supported by Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes.ReferenceProperty.");
    }

    private IBusinessObjectClass GetReferenceClassFromService ()
    {
      IBusinessObjectClassService service = GetBusinessObjectClassService ();
      IBusinessObjectClass businessObjectClass = service.GetBusinessObjectClass (UnderlyingType);
      if (businessObjectClass == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The GetBusinessObjectClass method of '{0}', registered with the '{1}', failed to return an '{2}' for type '{3}'.",
                service.GetType ().FullName,
                BusinessObjectProvider.GetType ().FullName,
                typeof (IBusinessObjectClass).FullName,
                UnderlyingType.FullName));
      }

      return businessObjectClass;
    }

    private IBusinessObjectClassService GetBusinessObjectClassService ()
    {
      IBusinessObjectClassService service = (IBusinessObjectClassService) BusinessObjectProvider.GetService (typeof (IBusinessObjectClassService));
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' type does not use the '{1}' implementation of '{2}' and there is no '{3}' registered with the '{4}'.",
                UnderlyingType.FullName,
                typeof (BindableDomainObject).Namespace,
                typeof (IBusinessObject).FullName,
                typeof (IBusinessObjectClassService).FullName,
                BusinessObjectProvider.GetType ().FullName));
      }
      return service;
    }
  }
}