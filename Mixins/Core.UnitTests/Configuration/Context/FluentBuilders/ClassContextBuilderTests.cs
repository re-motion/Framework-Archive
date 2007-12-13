using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.FluentBuilders
{
  [TestFixture]
  public class ClassContextBuilderTests
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _parentBuilderMock;
    private ClassContextBuilder _classBuilder;
    
    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _parentBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration)null);
      _classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), null);
    }

    [Test]
    public void Initialization_WithNoParentContext ()
    {
      Assert.AreSame (typeof (BaseType2), _classBuilder.TargetType);
      Assert.AreSame (_parentBuilderMock, _classBuilder.Parent);
      Assert.IsNull (_classBuilder.ParentContext);
      Assert.That (_classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (_classBuilder.CompleteInterfaces, Is.Empty);
      Assert.That (_classBuilder.TypesToInheritFrom, Is.Empty);

      ClassContext classContext = _classBuilder.BuildClassContext(new MixinConfiguration (null));
      Assert.AreEqual (0, classContext.MixinCount);
      Assert.AreEqual (0, classContext.CompleteInterfaceCount);
    }

    [Test]
    public void Initialization_WithParentContext ()
    {
      ClassContext existingClassContext = new ClassContext(typeof (BaseType1));
      existingClassContext.AddMixin (typeof (BT1Mixin1));

      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType1), existingClassContext);
      Assert.AreSame (existingClassContext, classBuilder.ParentContext);
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (classBuilder.CompleteInterfaces, Is.Empty);
      Assert.That (classBuilder.TypesToInheritFrom, Is.Empty);

      ClassContext classContext = classBuilder.BuildClassContext (new MixinConfiguration (null));
      Assert.AreEqual (1, classContext.MixinCount);
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
    }

    [Test]
    public void IgnoreParent ()
    {
      ClassContext existingClassContext = new ClassContext (typeof (BaseType1));
      existingClassContext.AddMixin (typeof (BT1Mixin1));

      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType1), existingClassContext);
      Assert.IsNotNull (classBuilder.ParentContext);

      Assert.AreSame (classBuilder, classBuilder.IgnoreParent());
      Assert.IsNull (classBuilder.ParentContext);
    }

    [Test]
    public void AddMixin_NonGeneric ()
    {
      MixinContextBuilder mixinBuilder = _classBuilder.AddMixin (typeof (BT2Mixin1));
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilder.MixinType);
      Assert.AreSame (_classBuilder, mixinBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, List.Contains (mixinBuilder));
    }

    [Test]
    public void AddMixin_Generic ()
    {
      MixinContextBuilder mixinBuilder = _classBuilder.AddMixin (typeof (BT2Mixin1));
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilder.MixinType);
      Assert.AreSame (_classBuilder, mixinBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, List.Contains (mixinBuilder));
    }

    [Test]
    public void AddMixins_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1)));
      Type[] mixinTypes = EnumerableUtility.ToArray (EnumerableUtility.Select<MixinContextBuilder, Type> (_classBuilder.MixinContextBuilders,
          delegate (MixinContextBuilder mcb) { return mcb.MixinType; }));
      Assert.That (mixinTypes, Is.EqualTo (new object[] {typeof (BT2Mixin1), typeof (BT3Mixin1)}));
    }

    [Test]
    public void AddMixins_Generic2 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddMixins<BT2Mixin1, BT3Mixin1>());
      Type[] mixinTypes = EnumerableUtility.ToArray (EnumerableUtility.Select<MixinContextBuilder, Type> (_classBuilder.MixinContextBuilders,
          delegate (MixinContextBuilder mcb) { return mcb.MixinType; }));
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1), typeof (BT3Mixin1) }));
    }

    [Test]
    public void AddMixins_Generic3 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      Type[] mixinTypes = EnumerableUtility.ToArray (EnumerableUtility.Select<MixinContextBuilder, Type> (_classBuilder.MixinContextBuilders,
          delegate (MixinContextBuilder mcb) { return mcb.MixinType; }));
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2) }));
    }

    [Test]
    public void AddOrderedMixins_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      List<MixinContextBuilder> mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (3, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That (mixinBuilders[1].Dependencies, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
      Assert.AreSame (typeof (BT3Mixin2), mixinBuilders[2].MixinType);
      Assert.That (mixinBuilders[2].Dependencies, Is.EqualTo (new object[] { typeof (BT3Mixin1) }));
    }

    [Test]
    public void AddOrderedMixins_Generic2 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins<BT2Mixin1, BT3Mixin1>());
      List<MixinContextBuilder> mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (2, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That (mixinBuilders[1].Dependencies, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
    }

    [Test]
    public void AddOrderedMixins_Generic3 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2>());
      List<MixinContextBuilder> mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (3, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That (mixinBuilders[1].Dependencies, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
      Assert.AreSame (typeof (BT3Mixin2), mixinBuilders[2].MixinType);
      Assert.That (mixinBuilders[2].Dependencies, Is.EqualTo (new object[] { typeof (BT3Mixin1) }));
    }

    [Test]
    public void AddCompleteInterface_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterface (typeof (IBT6Mixin1)));
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1) }));
    }

    [Test]
    public void AddCompleteInterface_Generic ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterface<IBT6Mixin1>());
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1) }));
    }

    [Test]
    public void AddCompleteInterfaces_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterfaces (typeof (IBT6Mixin1), typeof (IBT6Mixin2)));
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1), typeof (IBT6Mixin2) }));
    }

    [Test]
    public void AddCompleteInterfaces_Generic2 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2>());
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1), typeof (IBT6Mixin2) }));
    }

    [Test]
    public void AddCompleteInterfaces_Generic3 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ());
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1), typeof (IBT6Mixin2), typeof (IBT6Mixin3) }));
    }

    [Test]
    public void InheritFrom_Context ()
    {
      ClassContext inheritedContext1 = new ClassContext (typeof (BaseType1));
      ClassContext inheritedContext2 = new ClassContext (typeof (BaseType2));
      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom (inheritedContext1));
      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom (inheritedContext2));
      Assert.That (_classBuilder.TypesToInheritFrom, Is.EqualTo (new ClassContext[] { inheritedContext1, inheritedContext2 }));
    }

    [Test]
    public void InheritFrom_NonGeneric ()
    {
      _mockRepository.BackToRecordAll();

      ClassContext inheritedContext1 = new ClassContext (typeof (BaseType7));
      ClassContext inheritedContext2 = new ClassContext (typeof (BaseType6));

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (inheritedContext1);
      parentConfiguration.AddClassContext (inheritedContext2);

      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);
      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);

      _mockRepository.ReplayAll();

      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom (typeof (BaseType7)));
      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom (typeof (BaseType6)));

      Assert.That (_classBuilder.TypesToInheritFrom, Is.EqualTo (new ClassContext[] { inheritedContext1, inheritedContext2 }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void InheritFrom_NonGeneric_TypeWithNoContext ()
    {
      _mockRepository.BackToRecordAll();
      
      MixinConfiguration parentConfiguration = new MixinConfiguration (null);

      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);

      _mockRepository.ReplayAll();

      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom (typeof (BaseType7)));

      Assert.That (_classBuilder.TypesToInheritFrom, Is.Empty);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void InheritFrom_Generic ()
    {
      _mockRepository.BackToRecordAll ();

      ClassContext inheritedContext1 = new ClassContext (typeof (BaseType7));
      ClassContext inheritedContext2 = new ClassContext (typeof (BaseType6));

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (inheritedContext1);
      parentConfiguration.AddClassContext (inheritedContext2);

      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);
      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);

      _mockRepository.ReplayAll ();

      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom<BaseType7> ());
      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom<BaseType6> ());

      Assert.That (_classBuilder.TypesToInheritFrom, Is.EqualTo (new ClassContext[] { inheritedContext1, inheritedContext2 }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void InheritFrom_Generic_TypeWithNoContext ()
    {
      _mockRepository.BackToRecordAll ();

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);

      Expect.Call (_parentBuilderMock.ParentConfiguration).Return (parentConfiguration);

      _mockRepository.ReplayAll ();

      Assert.AreSame (_classBuilder, _classBuilder.InheritFrom<BaseType7>());

      Assert.That (_classBuilder.TypesToInheritFrom, Is.Empty);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void BuildContext_NoInheritance ()
    {
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2>();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2>();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (null);
      ClassContext builtContext = _classBuilder.BuildClassContext (mixinConfiguration);
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));
      
      Assert.AreEqual (2, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));

      Assert.AreEqual (2, builtContext.CompleteInterfaceCount);
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin2)));
    }

    [Test]
    public void BuildContext_WithInheritance ()
    {
      ClassContext inheritedContext = new ClassContext (typeof (BaseType7));
      inheritedContext.AddMixin (typeof (BT7Mixin1));
      
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ();
      _classBuilder.InheritFrom (inheritedContext);

      MixinConfiguration mixinConfiguration = new MixinConfiguration (null);
      ClassContext builtContext = _classBuilder.BuildClassContext (mixinConfiguration);
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));

      Assert.AreEqual (3, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT7Mixin1)));

      Assert.AreEqual (2, builtContext.CompleteInterfaceCount);
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin2)));
    }

    [Test]
    public void ParentMembers ()
    {
      _mockRepository.BackToRecordAll();
      
      ClassContextBuilder r1 = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (object), null);
      MixinConfiguration r2 = new MixinConfiguration (null);
      IDisposable r3 = _mockRepository.CreateMock<IDisposable> ();

      using (_mockRepository.Ordered ())
      {
        Expect.Call (_parentBuilderMock.ForClass (typeof (object))).Return (r1);
        Expect.Call (_parentBuilderMock.ForClass<string>()).Return (r1);
        Expect.Call (_parentBuilderMock.BuildConfiguration()).Return (r2);
        Expect.Call (_parentBuilderMock.EnterScope()).Return (r3);
      }
      
      _mockRepository.ReplayAll ();
      
      Assert.AreSame (r1, _classBuilder.ForClass (typeof (object)));
      Assert.AreSame (r1, _classBuilder.ForClass<string> ());
      Assert.AreSame (r2, _classBuilder.BuildConfiguration ());
      Assert.AreSame (r3, _classBuilder.EnterScope ());

      _mockRepository.VerifyAll ();
    }
  }
}