using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [Instantiable]
  [Serializable]
  public abstract class ClassDerivedFromSimpleDomainObject : SimpleDomainObject<ClassDerivedFromSimpleDomainObject>
  {
    public abstract int IntProperty { get; set; }
  }
}