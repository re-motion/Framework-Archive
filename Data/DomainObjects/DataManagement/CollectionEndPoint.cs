using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
{
public class CollectionEndPoint : RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  private DomainObjectCollection _domainObjects;

  // construction and disposing

  public CollectionEndPoint (DomainObjectCollection domainObjects, RelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    base.Initialize (definition);
    _domainObjects = domainObjects;
  }

  // methods and properties

  public DomainObjectCollection DomainObjects 
  {
    get { return _domainObjects; }
  }
}
}
