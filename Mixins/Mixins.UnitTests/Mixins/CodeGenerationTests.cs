using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using Mixins.UnitTests.Mixins.CodeGenSampleTypes;
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
      Type t = TypeFactory.Current.GetConcreteType (typeof (BaseType1));
      Assert.IsTrue (typeof (BaseType1).IsAssignableFrom (t));
      Assert.AreNotEqual (typeof (BaseType1), t);

      t = TypeFactory.Current.GetConcreteType (typeof (BaseType2));
      Assert.IsTrue (typeof (BaseType2).IsAssignableFrom (t));

      t = TypeFactory.Current.GetConcreteType (typeof (BaseType3));
      Assert.IsTrue (typeof (BaseType3).IsAssignableFrom (t));

      t = TypeFactory.Current.GetConcreteType (typeof (BaseType4));
      Assert.IsTrue (typeof (BaseType4).IsAssignableFrom (t));

      t = TypeFactory.Current.GetConcreteType (typeof (BaseType5));
      Assert.IsTrue (typeof (BaseType5).IsAssignableFrom (t));

      Assert.IsNotNull (ObjectFactory.Create<BaseType1> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType2> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType3> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType4> ());
      Assert.IsNotNull (ObjectFactory.Create<BaseType5> ());
    }

    [Uses(typeof (NullMixin))]
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
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin2)).With ();
      BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> ((object) bt3);
      Assert.IsNotNull (mixin);
      Assert.AreSame (bt3, mixin.This);
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3>(typeof (BT3Mixin1)).With ();
      BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> ((object) bt3);
      Assert.IsNotNull (mixin);
      Assert.AreSame (bt3, mixin.This);
      Assert.IsNotNull (mixin.Base);
      Assert.AreNotSame (bt3, mixin.Base);
    }

    [Test]
    public void GenericMixinsAreSpecialized ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>)).With ();
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), (object) bt3);
      Assert.IsNotNull (mixin);
      
      PropertyInfo thisProperty = mixin.GetType().BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (thisProperty);

      Assert.IsNotNull (thisProperty.GetValue (mixin, null));
      Assert.AreSame (bt3, thisProperty.GetValue (mixin, null));
      Assert.AreEqual (typeof (BaseType3), thisProperty.PropertyType);

      PropertyInfo baseProperty = mixin.GetType ().BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (baseProperty);

      Assert.IsNotNull (baseProperty.GetValue (mixin, null));
      Assert.AreEqual (bt3.GetType().GetNestedType("BaseCallProxy"), baseProperty.PropertyType);
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
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithPropsEventAtts)).With ();

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
    public void CompleteFaceInterface()
    {
      ICBaseType3BT3Mixin4 complete = CreateMixedObject<BaseType3> (typeof (BT3Mixin7Face), typeof (BT3Mixin4)).With () as ICBaseType3BT3Mixin4;

      Assert.IsNotNull (complete);
      Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType33) complete).IfcMethod ());
      Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType34) complete).IfcMethod ());
      Assert.AreEqual ("BaseType3.IfcMethod2", ((IBaseType35) complete).IfcMethod2 ());
      Assert.AreEqual ("BaseType3.IfcMethod-BT3Mixin4.Foo", Mixin.Get<BT3Mixin7Face> ((object) complete).InvokeThisMethods());
    }

    [Test]
    public void OverrideWithCompleteBaseInterface ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3>(typeof (BT3Mixin7Base), typeof (BT3Mixin4)).With();
      Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod());
    }

    [Test]
    public void OverrideMixinMethod ()
    {
      ClassOverridingMixinMethod com = CreateMixedObject<ClassOverridingMixinMethod> (typeof (AbstractMixin)).With();
      IAbstractMixin comAsIAbstractMixin = com as IAbstractMixin;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("AbstractMixin.ImplementedMethod-ClassOverridingMixinMethod.AbstractMethod-25", comAsIAbstractMixin.ImplementedMethod());
    }

    [Test]
    public void DoubleOverride ()
    {
      ClassOverridingMixinMethod com = CreateMixedObject<ClassOverridingMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIAbstractMixin = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("ClassOverridingMixinMethod.AbstractMethod-25", comAsIAbstractMixin.AbstractMethod(25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithExplicitImplementation)).With();
      IExplicit explicito = bt1 as IExplicit;
      Assert.IsNotNull (explicito);
      Assert.AreEqual ("XXX", explicito.Explicit());
    }

    [Test]
    [Ignore ("TODO: Configuration of generic mixins must be fixed up before generating code based on them.")]
    public void MixinCanIntroduceGenericInterface ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>)).With ();
      IGeneric<BaseType1> generic = bt1 as IGeneric<BaseType1>;
      Assert.IsNotNull (generic);
      Assert.AreEqual ("Generic", generic.Generic (bt1));
    }

    [Test]
    [Ignore ("TODO: Configuration of generic mixins must be fixed up before generating code based on them.")]
    public void MuchGenericityWithoutOverriding ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (VeryGenericMixin<,>), typeof (BT3Mixin4)).With();
      IVeryGenericMixin m = bt3 as IVeryGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage("5"));
    }

    [Test]
    [Ignore ("TODO: Configuration of generic mixins must be fixed up before generating code based on them.")]
    public void MuchGenericityWithOverriding ()
    {
      ClassOverridingUltraGenericStuff cougs = CreateMixedObject<ClassOverridingUltraGenericStuff> (typeof (AbstractDerivedUltraGenericMixin<,>),
          typeof(BT3Mixin4)).With ();
      IUltraGenericMixin m = cougs as IUltraGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("string-IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
    }

    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingInheritedInterface)).With();
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII1) bt1).Method1());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII2) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII2) bt1).Method2 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method3", ((IMixinIII3) bt1).Method3 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method4", ((IMixinIII4) bt1).Method4 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII4) bt1).Method2 ());
    }
  }
}
