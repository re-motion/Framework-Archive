using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithGuidKey : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithGuidKey GetObject (ObjectID id)
  {
    return (ClassWithGuidKey) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithGuidKey ()
  {
  }

  public ClassWithGuidKey (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithGuidKey (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public ClassWithValidRelations ClassWithValidRelationsOptional
  {
    get 
    { 
      return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsOptional"); 
    }
    set 
    { 
      SetRelatedObject ("ClassWithValidRelationsOptional", value); 
    }
  }

  public ClassWithValidRelations ClassWithValidRelationsNonOptional
  {
    get
    {
      return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsNonOptional");
    }
    set 
    { 
      SetRelatedObject ("ClassWithValidRelationsNonOptional", value); 
    }
  }

  public ClassWithInvalidRelation ClassWithInvalidRelation
  {
    get
    {
      return (ClassWithInvalidRelation) GetRelatedObject ("ClassWithInvalidRelation");
    }
    set 
    { 
      SetRelatedObject ("ClassWithInvalidRelation", value); 
    }
  }
}
}
