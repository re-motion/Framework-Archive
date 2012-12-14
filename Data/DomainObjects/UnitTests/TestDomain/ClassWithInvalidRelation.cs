using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithInvalidRelation : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithInvalidRelation GetObject (ObjectID id)
  {
    return (ClassWithInvalidRelation) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithInvalidRelation ()
  {
  }

  public ClassWithInvalidRelation (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithInvalidRelation (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public ClassWithGuidKey ClassWithGuidKey
  {
    get
    {
      return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKey");
    }
    set 
    { 
      SetRelatedObject ("ClassWithGuidKey", value); 
    }
  }
}
}
