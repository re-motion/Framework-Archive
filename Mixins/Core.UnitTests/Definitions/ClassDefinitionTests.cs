using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Remotion.Mixins.UnitTests.Definitions
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
      TargetClassDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (Derived));
      Assert.IsFalse (def.Methods.ContainsKey (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (def.Methods.ContainsKey (typeof (Base).GetMethod ("Foo")));

      InterfaceMapping mapping = def.GetAdjustedInterfaceMap(typeof (Interface));
      Assert.AreEqual (def.Methods[typeof (Base).GetMethod ("Foo")].MethodInfo, mapping.TargetMethods[0]);
    }

    [Test]
    public void GetAllMethods ()
    {
      TargetClassDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1));
      List<string> methodNames = new List<MethodDefinition> (def.GetAllMethods ()).ConvertAll<string> (
          delegate (MethodDefinition d) { return d.Name; });

      Assert.AreEqual (2, methodNames.FindAll (delegate (string s) { return s == "VirtualMethod"; }).Count);

      Assert.Contains ("get_VirtualProperty", methodNames);
      Assert.Contains ("set_VirtualProperty", methodNames);
      Assert.Contains ("get_Item", methodNames);
      Assert.Contains ("set_Item", methodNames);
      Assert.Contains ("add_VirtualEvent", methodNames);
      Assert.Contains ("remove_VirtualEvent", methodNames);
      Assert.Contains ("add_ExplicitEvent", methodNames);
      Assert.Contains ("remove_ExplicitEvent", methodNames);
      Assert.Contains ("ToString", methodNames);
      Assert.Contains ("Equals", methodNames);
      Assert.Contains ("GetHashCode", methodNames);
      Assert.Contains ("MemberwiseClone", methodNames);
      Assert.Contains ("GetType", methodNames);
      Assert.Contains ("Finalize", methodNames);

      Assert.AreEqual (16, methodNames.Count);

    }
  }
}