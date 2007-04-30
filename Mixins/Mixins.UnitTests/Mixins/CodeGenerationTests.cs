using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.UnitTests.SampleTypes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Mixins.Validation;

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
      public double Named;

      public ReplicatableAttribute(int i)
      {
        I = i;
      }

      public ReplicatableAttribute (string s)
      {
        S = s;
      }

      public double Named2
      {
        get
        {
          return Named;
        }
        set
        {
          Named = value;
        }
      }
    }

    [Replicatable(4)]
    public class MixinWithPropsEventAtts : IMixinWithPropsEventsAtts
    {
      private int _property; 

      [Replicatable("bla")]
      public int Property
      {
        [Replicatable(5, Named = 1.0)]
        get { return _property; }
        [Replicatable (5, Named2 = 2.0)]
        set { _property = value; }
      }

      [Replicatable("blo")]
      public event EventHandler Event
      {
        [Replicatable(1)]
        add {}
        [Replicatable (2)]
        remove { }
      }
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
    public void MixinsAreInitializedWithTarget ()
    {
      ObjectFactory factory = new ObjectFactory (new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin2))));
      BaseType3 bt3 = factory.Create<BaseType3>().With();
      BT3Mixin2 mixin = MixinReflectionHelper.GetMixinOf<BT3Mixin2> (bt3);
      Assert.IsNotNull (mixin);
      Assert.AreSame (bt3, mixin.This);
    }

    [Test]
    [Ignore ("Todo: Implement base")]
    public void MixinsAreInitializedWithBase ()
    {
      ObjectFactory factory = new ObjectFactory (new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin1))));
      BaseType3 bt3 = factory.Create<BaseType3> ().With ();
      BT3Mixin1 mixin = MixinReflectionHelper.GetMixinOf<BT3Mixin1> (bt3);
      Assert.IsNotNull (mixin);
      Assert.AreSame (bt3, mixin.This);
      Assert.IsNotNull (mixin.Base);
      Assert.AreNotSame (bt3, mixin.Base);
    }

    [Test]
    [Ignore("Todo: Implement base")]
    public void GenericMixinsAreSpecialized ()
    {
      ObjectFactory factory = new ObjectFactory (new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>))));
      BaseType3 bt3 = factory.Create<BaseType3> ().With ();
      object mixin = MixinReflectionHelper.GetMixinOf (typeof (BT3Mixin3<,>), bt3);
      Assert.IsNotNull (mixin);
      Assert.IsNotNull (mixin.GetType().GetField ("This").GetValue (mixin));
      Assert.IsNotNull (mixin.GetType ().GetField ("Base").GetValue (mixin));

      Assert.AreSame (bt3, mixin.GetType ().GetField ("This").GetValue (mixin));
      Assert.IsAssignableFrom (bt3.GetType().GetNestedType("BaseCallProxy"), mixin.GetType ().GetField ("Base").GetValue (mixin));
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
    public void PropertiesEventsAndAttributesAreReplicated()
    {
      TypeFactory = new TypeFactory (DefBuilder.Build (typeof (BaseType1), typeof (MixinWithPropsEventAtts)));
      ObjectFactory = new ObjectFactory (TypeFactory);

      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (BT1Attribute), false));
      Assert.IsTrue (bt1.GetType ().IsDefined (typeof (ReplicatableAttribute), false));

      ReplicatableAttribute[] atts = (ReplicatableAttribute[]) bt1.GetType().GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual (4, atts[0].I);

      PropertyInfo property = bt1.GetType().GetProperty (typeof (IMixinWithPropsEventsAtts).FullName + ".Property",
          BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (property);
      atts = (ReplicatableAttribute[]) property.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("bla", atts[0].S);
      Assert.IsTrue (property.GetGetMethod (true).IsSpecialName);
      atts = (ReplicatableAttribute[]) property.GetGetMethod (true).GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual( 1.0, atts[0].Named);

      Assert.IsTrue (property.GetSetMethod (true).IsSpecialName);
      atts = (ReplicatableAttribute[]) property.GetSetMethod (true).GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (2.0, atts[0].Named2);

      EventInfo eventInfo = bt1.GetType ().GetEvent (typeof (IMixinWithPropsEventsAtts).FullName + ".Event",
          BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (eventInfo);
      atts = (ReplicatableAttribute[]) eventInfo.GetCustomAttributes (typeof (ReplicatableAttribute), false);
      Assert.AreEqual (1, atts.Length);
      Assert.AreEqual ("blo", atts[0].S);
      Assert.IsTrue (eventInfo.GetAddMethod (true).IsSpecialName);
      Assert.IsTrue (eventInfo.GetAddMethod (true).IsDefined (typeof (ReplicatableAttribute), false));
      Assert.IsTrue (eventInfo.GetRemoveMethod (true).IsSpecialName);
      Assert.IsTrue (eventInfo.GetRemoveMethod (true).IsDefined (typeof (ReplicatableAttribute), false));
    }

    [Test]
    [Ignore("TODO: Implement base")]
    public void CompleteInterface()
    {
      TypeFactory = new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin7), typeof (BT3Mixin4)));
      ObjectFactory = new ObjectFactory (TypeFactory);

      ICBaseType3BT3Mixin4 complete = ObjectFactory.Create<BaseType3> ().With() as ICBaseType3BT3Mixin4;
      Assert.IsNotNull (complete);
      Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType34) complete).IfcMethod ());
      Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType35) complete).IfcMethod ());
    }
  }
}
