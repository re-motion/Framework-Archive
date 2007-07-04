using System;
using System.Reflection;
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
      get { return new DomainObjectClass ((IsList) ? ListInfo.ItemType : PropertyType); }
    }

    public IBusinessObject[] SearchAvailableObjects (bool requiresIdentity, IBusinessObject businessObject, string queryID)
    {
      if (queryID == null || queryID == string.Empty)
        return new IBusinessObjectWithIdentity[] { };

      QueryDefinition definition = QueryConfiguration.Current.QueryDefinitions.GetMandatory (queryID);
      if (definition.QueryType != QueryType.Collection)
        throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", queryID), "queryID");

      ClientTransaction clientTransaction = GetClientTransaction (businessObject);

      DomainObjectCollection result = clientTransaction.QueryManager.GetCollection (new Query (definition));
      IBusinessObjectWithIdentity[] availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.CopyTo (availableObjects, 0);

      return availableObjects;
    }

    public bool SupportsSearchAvailableObjects (bool requiresIdentity)
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

    private ClientTransaction GetClientTransaction (IBusinessObject businessObject)
    {
      DomainObject domainObject = businessObject as DomainObject;
      if (domainObject != null)
        return domainObject.InitialClientTransaction;
      else
        return ClientTransactionScope.CurrentTransaction;
    }
  }
}