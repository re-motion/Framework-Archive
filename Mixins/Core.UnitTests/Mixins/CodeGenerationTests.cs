using System;
using Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.UnitTests.Mixins
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
      private readonly int _i;
      private readonly string _s;
      private double _named;

      public ReplicatableAttribute(int i)
      {
        _i = i;
      }

      public ReplicatableAttribute (string s)
      {
        _s = s;
      }

      public int I
      {
        get { return _i; }
      }

      public string S
      {
        get { return _s; }
      }

      public double Named2
      {
        get
        {
          return _named;
        }
        set
        {
          _named = value;
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
        [Replicatable(5, Named2 = 1.0)]
        get { return _property; }
        [Replicatable (5, Named2 = 2.0)]
        set { _property = value; }
      }

      [Replicatable("blo")]
      public event EventHandler Event
      {
        [Replicatable(1)]
        add { }
        [Replicatable (2)]
        remove { }
      }
    }

    [Test]
    [ExpectedException(typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated1 ()
    {
      ObjectFactory.Create<ClassWithCtors> ().With ();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "constructor with signature", MatchType = MessageMatch.Contains)]
    public void ConstructorsAreReplicated2 ()
    {
      ObjectFactory.Create<ClassWithCtors> ().With (2.0);
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
      ClassWithCtors c = ObjectFactory.Create<ClassWithCtors>().With ("a");
      Assert.AreEqual ("a", c.O);
    }

    [Test]
    public void ConstructorsAreReplicated5 ()
    {
      NullMixin nullMixin = new NullMixin();
      ClassWithCtors c = ObjectFactory.CreateWithMixinInstances <ClassWithCtors> (nullMixin).With ("a");
      Assert.AreEqual ("a", c.O);
      Assert.AreSame (nullMixin, Mixin.Get<NullMixin> (c));
    }

    [Test]
    public void GenericMixinsAreSpecialized ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>)).With ();
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.IsNotNull (mixin);
      
      PropertyInfo thisProperty = mixin.GetType().BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (thisProperty);

      Assert.IsNotNull (thisProperty.GetValue (mixin, null));
      Assert.AreSame (bt3, thisProperty.GetValue (mixin, null));
      Assert.AreEqual (typeof (BaseType3), thisProperty.PropertyType);

      PropertyInfo baseProperty = mixin.GetType ().BaseType.GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull (baseProperty);

      Assert.IsNotNull (baseProperty.GetValue (mixin, null));
      Assert.AreSame (bt3.GetType().GetField("__first").FieldType, baseProperty.GetValue (mixin, null).GetType());
      Assert.AreEqual (typeof (IBaseType33), baseProperty.PropertyType);
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
      Assert.AreEqual( 1.0, atts[0].Named2);

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

    class Foo1
    { }

    class Foo2
    { }

    [CompleteInterface (typeof (Foo1))]
    [CompleteInterface (typeof (Foo2))]
    [IgnoreForMixinConfiguration]
    interface IMultiFace
    {
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
    public void MixinCanIntroduceGenericInterface ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>)).With ();
      IGeneric<BaseType1> generic = bt1 as IGeneric<BaseType1>;
      Assert.IsNotNull (generic);
      Assert.AreEqual ("Generic", generic.Generic (bt1));
    }

    [Test]
    public void MuchGenericityWithoutOverriding ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (VeryGenericMixin<,>), typeof (BT3Mixin4)).With();
      IVeryGenericMixin m = bt3 as IVeryGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage("5"));
    }

    [Test]
    public void MuchGenericityWithOverriding ()
    {
      ClassOverridingUltraGenericStuff cougs = CreateMixedObject<ClassOverridingUltraGenericStuff> (typeof (AbstractDerivedUltraGenericMixin<,>),
          typeof(BT3Mixin4)).With ();
      IUltraGenericMixin m = cougs as IUltraGenericMixin;
      Assert.IsNotNull (m);
      Assert.AreEqual ("String-IVeryGenericMixin.GenericIfcMethod-5", m.GetMessage ("5"));
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

    [Test]
    public void MixinImplementingFullPropertiesWithPartialIntroduction()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (MixinImplementingFullPropertiesWithPartialIntroduction)))
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
        MethodInfo[] allMethods = bt1.GetType().GetMethods (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        string[] allMethodNames = Array.ConvertAll<MethodInfo, string> (allMethods, delegate (MethodInfo mi) { return mi.Name; });
        Assert.That (allMethodNames, List.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.get_Prop1"));
        Assert.That (allMethodNames, List.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.set_Prop2"));
        
        Assert.That (allMethodNames, List.Not.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.set_Prop1"));
        Assert.That (allMethodNames, List.Not.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.get_Prop2"));
      }
    }

    [Test]
    public void TestMultipleOverridesSmall()
    {
      BaseType7 bt7 = ObjectFactory.Create<BaseType7> ().With();
      Assert.AreEqual ("BT7Mixin0.One(5)-BT7Mixin2.One(5)"
          + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
          + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
          + "-BaseType7.Two-BT7Mixin2.Two",
          bt7.One (5));

      Assert.AreEqual ("BT7Mixin0.One(foo)-BT7Mixin2.One(foo)"
          + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
          + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
          + "-BaseType7.Two-BT7Mixin2.Two",
          bt7.One ("foo"));

      Assert.AreEqual ("BT7Mixin2.Two", bt7.Two ());
      Assert.AreEqual ("BT7Mixin2.Three-BaseType7.Three", bt7.Three ());
      Assert.AreEqual ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BaseType7.Five-BaseType7.NotOverridden", bt7.Four ());
      Assert.AreEqual ("BT7Mixin9.Five-BaseType7.Five", bt7.Five ());
    }

    [Test]
    public void TestMultipleOverridesGrand()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType7), typeof (BT7Mixin0), typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3), typeof (BT7Mixin4), typeof (BT7Mixin5), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin10)))
      {
        MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).GetOrAddMixinContext (typeof (BT7Mixin0)).AddExplicitDependency (
            typeof (IBT7Mixin7));
        MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).GetOrAddMixinContext (typeof (BT7Mixin7)).AddExplicitDependency (
            typeof (IBT7Mixin4));
        MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).GetOrAddMixinContext (typeof (BT7Mixin4)).AddExplicitDependency (
            typeof (IBT7Mixin6));
        MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).GetOrAddMixinContext (typeof (BT7Mixin6)).AddExplicitDependency (
            typeof (IBT7Mixin2));
        MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).GetOrAddMixinContext (typeof (BT7Mixin9)).AddExplicitDependency (
            typeof (IBT7Mixin8));

        BaseType7 bt7 = ObjectFactory.Create<BaseType7>().With();
        Assert.AreEqual ("BT7Mixin0.One(7)-BT7Mixin4.One(7)-BT7Mixin6.One(7)-BT7Mixin2.One(7)"
            + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(7)-BaseType7.One(7)"
            + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(7)-BaseType7.One(7)"
            + "-BaseType7.Two"
            + "-BT7Mixin2.Two",
            bt7.One(7));

        Assert.AreEqual ("BT7Mixin0.One(bar)-BT7Mixin4.One(bar)-BT7Mixin6.One(bar)-BT7Mixin2.One(bar)"
            + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
            + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
            + "-BaseType7.Two"
            + "-BT7Mixin2.Two",
            bt7.One ("bar"));

        Assert.AreEqual ("BT7Mixin2.Two", bt7.Two());
        Assert.AreEqual ("BT7Mixin2.Three-BaseType7.Three", bt7.Three());
        Assert.AreEqual ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five-BaseType7.NotOverridden", bt7.Four ());
        Assert.AreEqual ("BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five", bt7.Five ());
      }
    }
  }
}
