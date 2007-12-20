using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.FluentBuilders
{
  [TestFixture]
  public class MixinContextBuilderTests
  {
    private MockRepository _mockRepository;
    private ClassContextBuilder _parentBuilderMock;
    private MixinContextBuilder _mixinBuilder;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _parentBuilderMock = _mockRepository.CreateMock<ClassContextBuilder> (new MixinConfigurationBuilder (null), typeof (object), null);
      _mixinBuilder = new MixinContextBuilder (_parentBuilderMock, typeof (BT2Mixin1));
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (typeof (BT2Mixin1), _mixinBuilder.MixinType);
      Assert.AreSame (_parentBuilderMock, _mixinBuilder.Parent);
      Assert.That (_mixinBuilder.Dependencies, Is.Empty);

      ClassContext classContext = new ClassContext (typeof (BaseType2));
      MixinContext mixinContext = _mixinBuilder.BuildMixinContext (classContext);
      Assert.AreEqual (0, mixinContext.ExplicitDependencyCount);
    }

    [Test]
    public void WithDependency_NonGeneric ()
    {
      _mixinBuilder.WithDependency (typeof (BT1Mixin1));
      Assert.That (_mixinBuilder.Dependencies, Is.EqualTo (new object[] { typeof (BT1Mixin1) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin Rubicon.Mixins.UnitTests.SampleTypes.BT2Mixin1 already has a "
        + "dependency on type Rubicon.Mixins.UnitTests.SampleTypes.BT1Mixin1.", MatchType = MessageMatch.Contains)]
    public void WithDependency_Twice ()
    {
      _mixinBuilder.WithDependency (typeof (BT1Mixin1)).WithDependency (typeof (BT1Mixin1));
    }

    [Test]
    public void WithDependency_Generic ()
    {
      _mixinBuilder.WithDependency<BT1Mixin1> ();
      Assert.That (_mixinBuilder.Dependencies, Is.EqualTo (new object[] { typeof (BT1Mixin1) }));
    }

    [Test]
    public void WithDependencies_NonGeneric ()
    {
      _mixinBuilder.WithDependencies (typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.That (_mixinBuilder.Dependencies, Is.EqualTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic2 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2>();
      Assert.That (_mixinBuilder.Dependencies, Is.EqualTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void WithDependencies_Generic3 ()
    {
      _mixinBuilder.WithDependencies<BT1Mixin1, BT1Mixin2, BT2Mixin1> ();
      Assert.That (_mixinBuilder.Dependencies, Is.EqualTo (new object[] { typeof (BT1Mixin1), typeof (BT1Mixin2), typeof (BT2Mixin1) }));
    }

    [Test]
    public void BuildContext ()
    {
      ClassContext classContext = new ClassContext (typeof (object));
      _mixinBuilder.WithDependency<IBT3Mixin4>();
      MixinContext context = _mixinBuilder.BuildMixinContext (classContext);
      Assert.AreSame (typeof (BT2Mixin1), context.MixinType);
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (context.ContainsExplicitDependency (typeof (IBT3Mixin4)));
    }

    [Test]
    public void ParentMembers ()
    {
      _mockRepository.BackToRecordAll ();

      ClassContextBuilder r1 = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (object), null);
      MixinConfiguration r2 = new MixinConfiguration (null);
      IDisposable r3 = _mockRepository.CreateMock<IDisposable> ();
      MixinContextBuilder r4 = new MixinContextBuilder (r1, typeof (BT1Mixin1));
      ClassContext r5 = new ClassContext (typeof (object));

      using (_mockRepository.Ordered ())
      {
        Expect.Call (_parentBuilderMock.Clear ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddMixin (typeof (object))).Return (r4);
        Expect.Call (_parentBuilderMock.AddMixin<string> ()).Return (r4);
        Expect.Call (_parentBuilderMock.AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        Expect.Call (_parentBuilderMock.AddMixins<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ()).Return (r1);
        Expect.Call (_parentBuilderMock.EnsureMixin (typeof (object))).Return (r4);
        Expect.Call (_parentBuilderMock.EnsureMixin<string> ()).Return (r4);
        Expect.Call (_parentBuilderMock.EnsureMixins (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        Expect.Call (_parentBuilderMock.EnsureMixins<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        Expect.Call (_parentBuilderMock.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddOrderedMixins (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        Expect.Call (_parentBuilderMock.AddOrderedMixins<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterface (typeof (IBT6Mixin1))).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterface<IBT6Mixin1>()).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterfaces (typeof (IBT6Mixin1), typeof (IBT6Mixin2))).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ()).Return (r1);
        Expect.Call (_parentBuilderMock.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ()).Return (r1);
        Expect.Call (_parentBuilderMock.SuppressMixin (typeof (object))).Return (r1);
        Expect.Call (_parentBuilderMock.SuppressMixin<string> ()).Return (r1);
        Expect.Call (_parentBuilderMock.SuppressMixins (typeof (BT1Mixin1), typeof (BT1Mixin2))).Return (r1);
        Expect.Call (_parentBuilderMock.SuppressMixins<BT1Mixin1, BT1Mixin2> ()).Return (r1);
        Expect.Call (_parentBuilderMock.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ()).Return (r1);
        Expect.Call (_parentBuilderMock.InheritFrom (r5)).Return (r1);
        Expect.Call (_parentBuilderMock.InheritFrom (typeof (BaseType5))).Return (r1);
        Expect.Call (_parentBuilderMock.InheritFrom<BaseType5>()).Return (r1);
        Expect.Call (_parentBuilderMock.BuildClassContext (r2)).Return (r5);

        Expect.Call (_parentBuilderMock.ForClass<object> ()).Return (r1);
        Expect.Call (_parentBuilderMock.ForClass<string> ()).Return (r1);
        Expect.Call (_parentBuilderMock.BuildConfiguration ()).Return (r2);
        Expect.Call (_parentBuilderMock.EnterScope ()).Return (r3);
      }

      _mockRepository.ReplayAll ();

      Assert.AreSame (r1, _mixinBuilder.Clear ());
      Assert.AreSame (r4, _mixinBuilder.AddMixin (typeof (object)));
      Assert.AreSame (r4, _mixinBuilder.AddMixin<string> ());
      Assert.AreSame (r1, _mixinBuilder.AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)));
      Assert.AreSame (r1, _mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ());
      Assert.AreSame (r1, _mixinBuilder.AddMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ());
      Assert.AreSame (r4, _mixinBuilder.EnsureMixin (typeof (object)));
      Assert.AreSame (r4, _mixinBuilder.EnsureMixin<string> ());
      Assert.AreSame (r1, _mixinBuilder.EnsureMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)));
      Assert.AreSame (r1, _mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2> ());
      Assert.AreSame (r1, _mixinBuilder.EnsureMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ());
      Assert.AreSame (r1, _mixinBuilder.AddOrderedMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)));
      Assert.AreSame (r1, _mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2> ());
      Assert.AreSame (r1, _mixinBuilder.AddOrderedMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ());
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterface (typeof (IBT6Mixin1)));
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterface<IBT6Mixin1> ());
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterfaces (typeof (IBT6Mixin1), typeof (IBT6Mixin2)));
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ());
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ());
      Assert.AreSame (r1, _mixinBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2, IBT6Mixin3> ());
      Assert.AreSame (r1, _mixinBuilder.SuppressMixin (typeof (object)));
      Assert.AreSame (r1, _mixinBuilder.SuppressMixin<string> ());
      Assert.AreSame (r1, _mixinBuilder.SuppressMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)));
      Assert.AreSame (r1, _mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2> ());
      Assert.AreSame (r1, _mixinBuilder.SuppressMixins<BT1Mixin1, BT1Mixin2, BT3Mixin1> ());
      Assert.AreSame (r1, _mixinBuilder.InheritFrom (r5));
      Assert.AreSame (r1, _mixinBuilder.InheritFrom (typeof (BaseType5)));
      Assert.AreSame (r1, _mixinBuilder.InheritFrom<BaseType5>());
      Assert.AreSame (r5, _mixinBuilder.BuildClassContext (r2));

      Assert.AreSame (r1, _mixinBuilder.ForClass<object> ());
      Assert.AreSame (r1, _mixinBuilder.ForClass<string> ());
      Assert.AreSame (r2, _mixinBuilder.BuildConfiguration ());
      Assert.AreSame (r3, _mixinBuilder.EnterScope ());

      _mockRepository.VerifyAll ();
    }
  }
}