using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithInvalidRelation")]
  public class ClassWithInvalidRelation: TestDomainBase
  {
    // types

    // static members and constants

    public new static ClassWithInvalidRelation GetObject (ObjectID id)
    {
      return (ClassWithInvalidRelation) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithInvalidRelation()
    {
    }

    public ClassWithInvalidRelation (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected ClassWithInvalidRelation (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClassWithInvalidRelation", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public ClassWithGuidKey ClassWithGuidKey
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKey"); }
      set { SetRelatedObject ("ClassWithGuidKey", value); }
    }
  }
}