using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.Utilities.Serialization;
using Rubicon;

namespace Mixins.UnitTests.Utilities.Serialization
{
  [TestFixture]
  public class FixupTests
  {
    [Test]
    public void RegistryTests()
    {
      Assert.IsNull (SerializationFixupRegistry.GetPreparationFunction (typeof (object)));
      Assert.IsNull (SerializationFixupRegistry.GetFixupFunction (typeof (object)));

      Assert.IsNull (SerializationFixupRegistry.GetPreparationFunction (typeof (string)));
      Assert.IsNull (SerializationFixupRegistry.GetFixupFunction (typeof (string)));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (MethodInfo)));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (MethodInfo)));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (object).Assembly.GetType("System.Reflection.RuntimeMethodInfo")));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (object).Assembly.GetType ("System.Reflection.RuntimeMethodInfo")));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (PropertyInfo)));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (PropertyInfo)));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (EventInfo)));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (EventInfo)));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (Type)));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (Type)));

      Assert.IsNotNull (SerializationFixupRegistry.GetPreparationFunction (typeof (Func<int>)));
      Assert.IsNotNull (SerializationFixupRegistry.GetFixupFunction (typeof (Func<int>)));
    }

    [Test]
    public void MethodFixupTests()
    {
      MethodInfo m1 = typeof (object).GetMethod ("Equals", BindingFlags.Public | BindingFlags.Instance);
      Assert.AreEqual (m1, new MethodInfoFixupData (m1).GetMethodInfo());

      MethodInfo m2 = typeof (Console).GetMethod ("WriteLine", new Type[] {typeof (string), typeof (object[])});
      Assert.AreEqual (m2, new MethodInfoFixupData (m2).GetMethodInfo ());
    }

    [Test]
    public void PropertyFixupTests ()
    {
      PropertyInfo p1 = typeof (OrderedDictionary).GetProperty ("Item", new Type[] {typeof(int)});
      Assert.AreEqual (p1, new PropertyInfoFixupData (p1).GetPropertyInfo());

      PropertyInfo p2 = typeof (OrderedDictionary).GetProperty ("Item", new Type[] { typeof (string) });
      Assert.AreEqual (p2, new PropertyInfoFixupData (p2).GetPropertyInfo ());

      PropertyInfo p3 = typeof (OrderedDictionary).GetProperty ("Count");
      Assert.AreEqual (p3, new PropertyInfoFixupData (p3).GetPropertyInfo ());
    }

    [Test]
    public void EventInfoFixupTests ()
    {
      EventInfo e1 = typeof (AppDomain).GetEvent ("AssemblyLoad");
      Assert.AreEqual (e1, new EventInfoFixupData (e1).GetEventInfo ());

      EventInfo e2 = typeof (AppDomain).GetEvent ("AssemblyResolve");
      Assert.AreEqual (e2, new EventInfoFixupData (e2).GetEventInfo ());
    }

    [Test]
    public void TypeFixupTests ()
    {
      Type t1 = typeof (object);
      Assert.AreEqual (t1, new TypeFixupData (t1).GetTypeObject());

      Type t2 = typeof (Dictionary<,>.Enumerator);
      Assert.AreEqual (t2, new TypeFixupData (t2).GetTypeObject ());

      Type t3 = typeof (Dictionary<int,string>.Enumerator);
      Assert.AreEqual (t3, new TypeFixupData (t3).GetTypeObject ());
    }

    private static void StaticMethod (object sender, EventArgs e)
    {
      throw new NotImplementedException ();
    }

    private class FuncHolder
    {
      public string S;

      public string Func()
      {
        return S;
      }
    }

    [Test]
    public void DelegateFixupTests ()
    {
      EventHandler d1 = StaticMethod;
      Assert.AreEqual (d1, new DelegateFixupData (d1).GetDelegate());

      FuncHolder holder = new FuncHolder();
      holder.S = "Foo";

      Func<string> d2 = holder.Func;
      Assert.AreEqual (d2, new DelegateFixupData (d2).GetDelegate ());
      Assert.AreEqual ("Foo", ((Func<string>)new DelegateFixupData (d2).GetDelegate ())());
    }
  }
}
