using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.FluentBuilders
{
  [TestFixture]
  public class ClassContextBuilderTests
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _parentBuilderMock;
    private ClassContextBuilder _classBuilder;
    private ClassContextBuilder _classBuilderMock;
    private MixinContextBuilder _mixinBuilderMock;
    
    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _parentBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration)null);
      _classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), null);
      _classBuilderMock = _mockRepository.CreateMock<ClassContextBuilder> (_parentBuilderMock, typeof (BaseType2), null);
      _mixinBuilderMock = _mockRepository.CreateMock<MixinContextBuilder> (_classBuilderMock, typeof (BT2Mixin1));
    }

    private Type[] GetMixinTypes ()
    {
      return GetMixinTypes (_classBuilder);
    }

    private Type[] GetMixinTypes (ClassContextBuilder classBuilder)
    {
      return EnumerableUtility.ToArray (EnumerableUtility.Select<MixinContextBuilder, Type> (classBuilder.MixinContextBuilders,
          delegate (MixinContextBuilder mcb) { return mcb.MixinType; }));
    }

    [Test]
    public void Initialization_WithNoParentContext ()
    {
      Assert.AreSame (typeof (BaseType2), _classBuilder.TargetType);
      Assert.AreSame (_parentBuilderMock, _classBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (_classBuilder.CompleteInterfaces, Is.Empty);
      
      ClassContext classContext = _classBuilder.BuildClassContext(new MixinConfiguration (null), new ClassContext[0]);
      Assert.AreEqual (0, classContext.MixinCount);
      Assert.AreEqual (0, classContext.CompleteInterfaceCount);
    }

    [Test]
    public void Initialization_WithParentContext ()
    {
      ClassContext existingClassContext = new ClassContext(typeof (BaseType1));
      existingClassContext.AddMixin (typeof (BT1Mixin1));
      existingClassContext.AddCompleteInterface (typeof (IBT1Mixin1));

      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType1), existingClassContext);
      Assert.That (GetMixinTypes (classBuilder),
        Is.EqualTo (new object[] {typeof (BT1Mixin1)}));
      Assert.That (classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT1Mixin1) }));

      ClassContext classContext = classBuilder.BuildClassContext (new MixinConfiguration (null), new ClassContext[0]);
      Assert.AreEqual (1, classContext.MixinCount);
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.AreEqual (1, classContext.CompleteInterfaceCount);
      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (IBT1Mixin1)));
    }

    [Test]
    public void Clear ()
    {
      ClassContext existingClassContext = new ClassContext (typeof (BaseType1));
      existingClassContext.AddMixin (typeof (BT1Mixin1));

      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType1), existingClassContext);
      classBuilder.AddMixin<BT1Mixin2> ();
      classBuilder.AddCompleteInterface<IBaseType31> ();
      
      Assert.That (classBuilder.MixinContextBuilders, Is.Not.Empty);
      Assert.That (classBuilder.CompleteInterfaces, Is.Not.Empty);
      Assert.That (classBuilder.SuppressInheritance, Is.False);

      Assert.AreSame (classBuilder, classBuilder.Clear());
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (classBuilder.CompleteInterfaces, Is.Empty);
      Assert.That (classBuilder.SuppressInheritance, Is.True);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Rubicon.Mixins.UnitTests.SampleTypes.BT2Mixin1 is already configured as a "
        + "mixin for type Rubicon.Mixins.UnitTests.SampleTypes.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice ()
    {
      _classBuilder.AddMixin (typeof (BT2Mixin1)).AddMixin (typeof (BT2Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Rubicon.Mixins.UnitTests.SampleTypes.BT2Mixin1 is already configured as a "
        + "mixin for type Rubicon.Mixins.UnitTests.SampleTypes.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_ConflictWithParentContext ()
    {
      ClassContext parentContext = new ClassContext (typeof (BaseType2), typeof (BT2Mixin1));
      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);
      classBuilder.AddMixin (typeof (BT2Mixin1));
    }

    [Test]
    public void AddMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.AddMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_mixinBuilderMock, _classBuilderMock.AddMixin<BT2Mixin1>());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT3Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT3Mixin2))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1>())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
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
    public void EnsureMixin_NonGeneric ()
    {
      MixinContextBuilder builder = _classBuilder.EnsureMixin (typeof (BT2Mixin1));
      Assert.AreEqual (typeof (BT2Mixin1), builder.MixinType);
      Type[] mixinTypes = GetMixinTypes();
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
      Assert.AreSame (builder, _classBuilder.EnsureMixin (typeof (BT2Mixin1)));
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
    }

    [Test]
    public void EnsureMixin_Inheritance ()
    {
      ClassContext contextWithMixin = new ClassContext (typeof (BaseType3));
      contextWithMixin.AddMixin (typeof (NullTarget));
      
      MixinContextBuilder builder = _classBuilder.EnsureMixin (typeof (DerivedNullTarget));
      Assert.AreEqual (typeof (DerivedNullTarget), builder.MixinType);
      Type[] mixinTypes = GetMixinTypes ();
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (DerivedNullTarget) }));

      ClassContext builtContext = _classBuilder.BuildClassContext (new MixinConfiguration (null), new ClassContext[] {contextWithMixin});
      Assert.AreEqual (1, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (DerivedNullTarget)));
      Assert.IsFalse (builtContext.ContainsMixin (typeof (NullTarget)));
    }

    [Test]
    public void EnsureMixin_Parent ()
    {
      ClassContext parentContext = new ClassContext (typeof (BaseType3));
      parentContext.AddMixin (typeof (BT2Mixin1));
      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);

      Type[] mixinTypes = GetMixinTypes (classBuilder);
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));

      MixinContextBuilder builder = classBuilder.EnsureMixin (typeof (BT2Mixin1));
      Assert.AreEqual (typeof (BT2Mixin1), builder.MixinType);
      mixinTypes = GetMixinTypes (classBuilder);
      Assert.That (mixinTypes, Is.EqualTo (new object[] { typeof (BT2Mixin1) }));
    }

    [Test]
    public void EnsureMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.EnsureMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_mixinBuilderMock, _classBuilderMock.EnsureMixin<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT3Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT3Mixin2))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterface_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterface (typeof (IBT6Mixin1)));
      Assert.That (_classBuilder.CompleteInterfaces, Is.EqualTo (new object[] { typeof (IBT6Mixin1) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Rubicon.Mixins.UnitTests.SampleTypes.IBT6Mixin1 is already configured as a "
        + "complete interface for type Rubicon.Mixins.UnitTests.SampleTypes.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddCompleteInterface_Twice ()
    {
      _classBuilder.AddCompleteInterface (typeof (IBT6Mixin1)).AddCompleteInterface (typeof (IBT6Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Rubicon.Mixins.UnitTests.SampleTypes.IBT6Mixin1 is already configured as a "
        + "complete interface for type Rubicon.Mixins.UnitTests.SampleTypes.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddCompleteInterface_ConflictWithParentContext ()
    {
      ClassContext parentContext = new ClassContext (typeof (BaseType2));
      parentContext.AddCompleteInterface (typeof (IBT6Mixin1));

      ClassContextBuilder classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);
      classBuilder.AddCompleteInterface (typeof (IBT6Mixin1));
    }

    [Test]
    public void AddCompleteInterface_Generic ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterface<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT2Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterface<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT2Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT3Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_Generic2 ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_Generic3 ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixin_NonGeneric ()
    {
      Assert.That (_classBuilder.SuppressedMixins, Is.Empty);
      _classBuilder.SuppressMixin (typeof (BT1Mixin1));
      _classBuilder.SuppressMixin (typeof (BT2Mixin1));
      Assert.That (_classBuilder.SuppressedMixins, Is.EquivalentTo (new object[] { typeof (BT2Mixin1), typeof (BT1Mixin1) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin type Rubicon.Mixins.UnitTests.SampleTypes.BT2Mixin1 has already "
        + "been suppressed for target type Rubicon.Mixins.UnitTests.SampleTypes.BaseType2.", MatchType = MessageMatch.Contains)]
    public void SuppressMixin_Twice ()
    {
      _classBuilder.SuppressMixin (typeof (BT2Mixin1));
      _classBuilder.SuppressMixin (typeof (BT2Mixin1));
    }

    [Test]
    public void SuppressMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.SuppressMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT2Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixin<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT2Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT3Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void BuildContext_NoInheritance ()
    {
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2>();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2>();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (null);
      ClassContext builtContext = _classBuilder.BuildClassContext (mixinConfiguration, new ClassContext[0]);
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));
      
      Assert.AreEqual (2, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));

      Assert.AreEqual (2, builtContext.CompleteInterfaceCount);
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin2)));
    }

    [Test]
    public void BuildContext_SuppressedInheritance ()
    {
      ClassContext inheritedContext = new ClassContext (typeof (BaseType2), typeof (BT3Mixin1));
      inheritedContext.AddCompleteInterface (typeof (BT1Mixin2));

      _classBuilder.Clear ();
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (null);
      ClassContext builtContext = _classBuilder.BuildClassContext (mixinConfiguration, new ClassContext[] { inheritedContext });
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
      inheritedContext.AddCompleteInterface (typeof (BT1Mixin2));
      
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (null);
      ClassContext builtContext = _classBuilder.BuildClassContext (mixinConfiguration, new ClassContext[] { inheritedContext });
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));

      Assert.AreEqual (3, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT7Mixin1)));

      Assert.AreEqual (3, builtContext.CompleteInterfaceCount);
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (IBT6Mixin2)));
      Assert.IsTrue (builtContext.ContainsCompleteInterface (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_ExtendParentContext ()
    {
      ClassContext parentContext = new ClassContext (typeof (BaseType2));
      parentContext.AddMixin (typeof (BT2Mixin1));

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (parentContext);

      ClassContextBuilder classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);
      classContextBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (parentConfiguration);
      ClassContext builtContext = classContextBuilder.BuildClassContext (mixinConfiguration, new ClassContext[0]);
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));
      
      Assert.AreEqual (3, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_ReplaceParentContext ()
    {
      ClassContext parentContext = new ClassContext (typeof (BaseType2));
      parentContext.AddMixin (typeof (BT2Mixin1));

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (parentContext);

      ClassContextBuilder classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);
      classContextBuilder.Clear ().AddMixins<BT1Mixin1, BT1Mixin2> ();

      MixinConfiguration mixinConfiguration = new MixinConfiguration (parentConfiguration);
      ClassContext builtContext = classContextBuilder.BuildClassContext (mixinConfiguration, new ClassContext[0]);
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));

      Assert.AreEqual (2, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_Suppression ()
    {
      ClassContext inheritedContext = new ClassContext (typeof (BaseType2));
      inheritedContext.AddMixin (typeof (BT3Mixin1));
      inheritedContext.AddMixin (typeof (BT3Mixin3<IBaseType33, IBaseType33>));

      ClassContext parentContext = new ClassContext (typeof (BaseType2));
      parentContext.AddMixin (typeof (BT5Mixin1));
      parentContext.AddMixin (typeof (BT5Mixin2));

      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.AddClassContext (parentContext);

      ClassContextBuilder classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2), parentContext);
      classContextBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();

      classContextBuilder.SuppressMixins (typeof (IBT1Mixin1), typeof (BT5Mixin1), typeof (BT3Mixin3<,>));

      MixinConfiguration mixinConfiguration = new MixinConfiguration (parentConfiguration);
      ClassContext builtContext = classContextBuilder.BuildClassContext (mixinConfiguration, new ClassContext[] { inheritedContext });
      Assert.IsTrue (mixinConfiguration.ContainsClassContext (builtContext.Type));

      Assert.AreEqual (3, builtContext.MixinCount);
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT3Mixin1)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT5Mixin2)));
      Assert.IsTrue (builtContext.ContainsMixin (typeof (BT1Mixin2)));
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
        Expect.Call (_parentBuilderMock.ForClass<object> ()).Return (r1);
        Expect.Call (_parentBuilderMock.ForClass<string>()).Return (r1);
        Expect.Call (_parentBuilderMock.BuildConfiguration()).Return (r2);
        Expect.Call (_parentBuilderMock.EnterScope()).Return (r3);
      }
      
      _mockRepository.ReplayAll ();
      
      Assert.AreSame (r1, _classBuilder.ForClass<object> ());
      Assert.AreSame (r1, _classBuilder.ForClass<string> ());
      Assert.AreSame (r2, _classBuilder.BuildConfiguration ());
      Assert.AreSame (r3, _classBuilder.EnterScope ());

      _mockRepository.VerifyAll ();
    }
  }
}