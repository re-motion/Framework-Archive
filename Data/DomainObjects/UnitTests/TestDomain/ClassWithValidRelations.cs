using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithValidRelations : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithValidRelations GetObject (ObjectID id)
  {
    return (ClassWithValidRelations) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithValidRelations ()
  {
  }

  public ClassWithValidRelations (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithValidRelations (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public ClassWithGuidKey ClassWithGuidKeyOptional
  {
    get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyOptional"); }
    set { SetRelatedObject ("ClassWithGuidKeyOptional", value); }
  }

  public ClassWithGuidKey ClassWithGuidKeyNonOptional
  {
    get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyNonOptional"); }
    set { SetRelatedObject ("ClassWithGuidKeyNonOptional", value); }
  }
}
}
