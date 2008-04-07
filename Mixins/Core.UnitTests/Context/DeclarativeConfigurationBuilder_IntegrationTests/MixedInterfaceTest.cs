using System;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixedInterfaceTest
  {
    [Test]
    public void MixedInterface_GetsClassContext_ViaUses ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void MixedInterface_GetsClassContext_ViaExtends ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void ImplementingClass_InheritsMixins ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void Definition_ForImplementingClass ()
    {
      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (ClassWithMixedInterface)).Mixins.ContainsKey (typeof (NullMixin)));
    }
  }
}