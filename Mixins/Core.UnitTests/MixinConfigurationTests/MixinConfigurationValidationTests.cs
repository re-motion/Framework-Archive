using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationValidationTests
  {
    [Test]
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
          Assert.IsTrue (log.GetNumberOfSuccesses () > 0);
          Assert.AreEqual (0, log.GetNumberOfFailures ());
        }
      }
    }

    [Test]
    public void ValidateWithErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<int> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    class UninstantiableGeneric<T>
      where T : ISerializable, IServiceProvider
    {
    }

    [Test]
    public void ValidateWithGenerics_IgnoresGenerics ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (KeyValuePair<,>)).Clear ().AddMixins (typeof (NullMixin))
          .ForClass (typeof (UninstantiableGeneric<>)).Clear().AddMixins (typeof (NullMixin))
          .EnterScope ())
      {
        IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
        Assert.AreEqual (0, log.GetNumberOfFailures ());
      }
    }
  }
}