using System;
using System.Reflection;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class ReferenceProperty : NullableProperty, IBusinessObjectReferenceProperty
{
  public ReferenceProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList)
      : base (propertyInfo, isRequired, itemType, isList, true)
  {
  }

  public IBusinessObjectClass ReferenceClass
  {
    get { return new DomainObjectClass ((IsList) ? ItemType : PropertyType); }
  }

  public IBusinessObjectWithIdentity[] SearchAvailableObjects (IBusinessObject obj, string queryID)
  {
    if (queryID == null || queryID == string.Empty)
      return new BindableDomainObject[] {};

    QueryDefinition definition = QueryConfiguration.Current.QueryDefinitions.GetMandatory (queryID);
    if (definition.QueryType != QueryType.Collection)
      throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", queryID), "queryID");

    DomainObjectCollection result = ClientTransaction.Current.QueryManager.GetCollection (new Query (definition));
    IBusinessObjectWithIdentity[] availableObjects = new IBusinessObjectWithIdentity[result.Count];
  
    if (availableObjects.Length > 0)
      result.CopyTo (availableObjects, 0);
    
    return availableObjects;
  }

  public bool SupportsSearchAvailableObjects
  {
    get { return true; }
  }
 
  public bool CreateIfNull 
  { 
    get { return false; } 
  }
  
  public IBusinessObject Create (IBusinessObject referencingObject)
  {
    throw new NotSupportedException ("Create method is not supported by Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes.ReferenceProperty.");
  }
}
}