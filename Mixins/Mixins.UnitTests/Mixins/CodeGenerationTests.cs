using System;
using System.Runtime.Serialization;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.UnitTests.SampleTypes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class CodeGenerationTests : MixinTestBase
  {
    [Test]
    public void GeneratedTypeIsAssignableButDifferent ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.IsTrue (typeof (BaseType1).IsAssignableFrom (t));
      Assert.AreNotEqual (typeof (BaseType1), t);

      t = TypeFactory.GetConcreteType (typeof (BaseType2));
      Assert.IsTrue (typeof (BaseType2).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (typeof (BaseType3).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType4));
      Assert.IsTrue (typeof (BaseType4).IsAssignableFrom (t));

      t = TypeFactory.GetConcreteType (typeof (BaseType5));
      Assert.IsTrue (typeof (BaseType5).IsAssignableFrom (t));

      Assert.IsNotNull (ObjectFactory.Create<BaseType1> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType2> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType3> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType4> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType5> ());
    }

    [ApplyMixin(typeof (NullMixin))]
    public class ClassWithCtors
    {
      public object O;

      public ClassWithCtors (int i)
      {
        O = i;
      }

      public ClassWithCtors (string s)
      {
        O = s;
      }
    }

    public interface IMixinWithPropsEventsAtts
    {
      int Property { get; set; }
      event EventHandler Event;
    }

    public class ReplicatableAttribute : Attribute
    {
      public readonly int I;
      public readonly string S;

      public ReplicatableAttribute(int i)
      {
        I = i;
      }

      public ReplicatableAttribute (string s)
      {
        S = s;
      }
    }

    [Replicatable(4)]
    public class MixinWithPropsEventAtts : IMixinWithPropsEventsAtts
    {
      private int _property; 

      [Replicatable("bla")]
      public int Property
      {
        get { return _property; }
        set { _property = value; }
      }

      [Replicatable("blo")]
      public event EventHandler Event;
    }

    [Test]
    [ExpectedException(typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated1 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With ();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated2 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With (2.0);
    }

    [Test]
    public void ConstructorsAreReplicated3 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With (3);
      Assert.AreEqual(3, c.O);
    }

    [Test]
    public void ConstructorsAreReplicated4 ()
    {
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors> ().With ("a");
      Assert.AreEqual ("a", c.O);
    }

    [Test]
    public void IMixinTarget()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IMixinTarget mixinTarget = bt1 as IMixinTarget;
      Assert.IsNotNull (mixinTarget);

      BaseClassDefinition configuration = mixinTarget.Configuration;
      Assert.IsNotNull (configuration);
      Assert.AreSame (Configuration.BaseClasses[typeof (BaseType1)], configuration);

      object[] mixins = mixinTarget.Mixins;
      Assert.IsNotNull (mixins);
      Assert.AreEqual (configuration.Mixins.Count, mixins.Length);
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      ObjectFactory factory = new ObjectFactory (new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin2))));
      BaseType3 bt3 = factory.Create<BaseType3>().With();
      BT3Mixin2 mixin = MixinReflectionHelper.GetMixinOf<BT3Mixin2> (bt3);
      Assert.IsNotNull (mixin);
      Assert.AreSame (bt3, mixin.This);
      Assert.IsNull (mixin.Base);
    }

    [Test]
    [Ignore("TODO")]
    public void MixinsAreInitializedWithBase ()
    {
      Assert.Fail();
    }

    [Test]
    public void IntroducedInterfacesAreImplementedViaDelegation ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IBT1Mixin1 bt1AsMixedIface = bt1 as IBT1Mixin1;
      Assert.IsNotNull (bt1AsMixedIface);
      Assert.AreEqual ("BT1Mixin1.IntroducedMethod", bt1AsMixedIface.IntroducedMethod());
    }

    [Test]
    [Ignore ("TODO - Add properties, events, and attributes to definition class structure and implement it in code generation.")]
    public void PropertiesEventsAndAttributesAreReplicated()
    {
      ObjectFactory factory = new ObjectFactory (new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin2))));
      BaseType1 bt1 = factory.Create<BaseType1>().With();
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (ReplicatableAttribute), false));
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (BT1Attribute), false));

      ReplicatableAttribute[] atts = (ReplicatableAttribute[]) bt1.GetType().GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual (4, atts[0].I);

      PropertyInfo property = bt1.GetType().GetProperty ("Property");
      Assert.IsNotNull (property);
      atts = (ReplicatableAttribute[]) property.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("Bla", atts[0].S);

      EventInfo eventInfo = bt1.GetType ().GetEvent ("Event");
      Assert.IsNotNull (eventInfo);
      atts = (ReplicatableAttribute[]) eventInfo.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("Blo", atts[0].S);
    }
  }
}
