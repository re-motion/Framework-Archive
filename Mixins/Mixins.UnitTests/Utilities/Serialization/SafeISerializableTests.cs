using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Utilities.Serialization;
using NUnit.Framework;
using System.Runtime.Serialization;
using Mixins.Utilities;
using Rubicon;

namespace Mixins.UnitTests.Utilities.Serialization
{
  [TestFixture]
  public class SafeISerializableTests
  {
    [Serializable]
    class Base : SafeISerializableBase
    {
      public int BaseOne;
      public string BaseTwo;

      public MethodInfo Method;
      public PropertyInfo Property;
      public EventInfo Event;
      public Type Type;

      public Func<int> Func;

      public Base () { }
      public Base (SerializationInfo info, StreamingContext context) : base (info, context) { }
    }

    [Serializable]
    class C : Base
    {
      public int One;
      public string Two;

      public C () { }
      public C (SerializationInfo info, StreamingContext context) : base (info, context) { }
    }

    [Test]
    public void ClassIsMadeISerializable()
    {
      Assert.IsTrue (typeof (ISerializable).IsAssignableFrom(typeof (C)));
    }

    [Test]
    public void SimpleMembersCorrectlySerialized()
    {
      C c = new C();
      c.One = 1;
      c.Two = "Two";

      C c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreEqual (1, c2.One);
      Assert.AreEqual ("Two", c2.Two);
    }

    [Test]
    public void BaseMembersCorrectlySerialized ()
    {
      C c = new C ();
      c.BaseOne = 1;
      c.BaseTwo = "Two";

      C c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreEqual (1, c2.BaseOne);
      Assert.AreEqual ("Two", c2.BaseTwo);
    }

    [Test]
    public void ReflectionTypesCorrectlySerialized()
    {
      C c = new C ();
      
      c.Method = typeof (object).GetMethod ("ToString");
      c.Property = typeof (DateTime).GetProperty ("Now");
      c.Event = typeof (AppDomain).GetEvent ("UnhandledException");
      c.Type = typeof (C);

      TestFunctionHolder h = new TestFunctionHolder();
      h.I = 17;
      c.Func = h.TestFunction;

      C c2 = Serializer.SerializeAndDeserialize (c);
      Assert.AreNotSame (c, c2);

      Assert.IsNotNull (c2.Method);
      Assert.AreEqual (c.Method, c2.Method);

      Assert.IsNotNull (c2.Property);
      Assert.AreEqual (c.Property, c2.Property);

      Assert.IsNotNull (c2.Event);
      Assert.AreEqual (c.Event, c2.Event);

      Assert.IsNotNull (c2.Type);
      Assert.AreEqual (c.Type, c2.Type);

      Assert.IsNotNull (c2.Func);
      Assert.AreEqual (c.Func (), c2.Func ());
    }

    [Serializable]
    class TestFunctionHolder
    {
      public int I;

      public int TestFunction ()
      {
        return I;
      }
    }
  }
}
