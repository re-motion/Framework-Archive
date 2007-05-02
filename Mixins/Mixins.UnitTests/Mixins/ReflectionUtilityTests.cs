using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mixins.CodeGeneration;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ReflectionUtilityTests
  {
    class GenericBase<T>
    {
      public virtual void Foo() { }
    }

    class GenericSub<T> : GenericBase<T>
    {
      public override void Foo () { }
    }

    class NonGenericSub : GenericSub<int>
    {
      public new void Foo () { }
    }

    [Test]
    public void FindGenericTypeDefinitionInClosedHierarchy()
    {
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (GenericBase<int>)));
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (GenericSub<int>)));
      Assert.AreEqual (typeof (GenericBase<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericBase<>),
        typeof (NonGenericSub)));

      Assert.AreEqual (typeof (GenericSub<int>), ReflectionUtility.FindGenericTypeDefinitionInClosedHierarchy (typeof (GenericSub<>),
        typeof (NonGenericSub)));
    }

    [Test]
    public void MapMethodInfoOfGenericTypeDefinitionToClosedHierarchy ()
    {
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName(typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (GenericBase<int>)));
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (GenericSub<int>)));
      Assert.AreEqual (typeof (GenericBase<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericBase<>).GetMethod ("Foo"),
          typeof (NonGenericSub)));

      Assert.AreEqual (typeof (GenericSub<int>).GetMethod ("Foo"),
          ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericSub<>).GetMethod ("Foo"),
          typeof (GenericSub<int>)));
      Assert.IsNull (ReflectionUtility.MapMethodInfoOfGenericTypeDefinitionToClosedHierarchyByName (typeof (GenericSub<>).GetMethod ("Foo"),
          typeof (GenericBase<int>)));
    }
  }
}
