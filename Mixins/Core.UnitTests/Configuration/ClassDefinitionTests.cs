using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ClassDefinitionTests
  {
    interface Interface
    {
      void Foo();
    }

    class Base
    {
      public void Foo() {}
    }

    class Derived : Base, Interface
    {
    }

    [Test]
    public void InterfaceMapAdjustedCorrectly ()
    {
      BaseClassDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (Derived));
      Assert.IsFalse (def.Methods.HasItem (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (def.Methods.HasItem (typeof (Base).GetMethod ("Foo")));

      InterfaceMapping mapping = def.GetAdjustedInterfaceMap(typeof (Interface));
      Assert.AreEqual (def.Methods[typeof (Base).GetMethod ("Foo")].MethodInfo, mapping.TargetMethods[0]);
    }
  }
}
