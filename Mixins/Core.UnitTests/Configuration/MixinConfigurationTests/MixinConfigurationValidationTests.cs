using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationValidationTests
  {
    [Test]
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
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
        using (MixinConfiguration.ScopedExtend (typeof (int), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    [Test]
    public void ValidateWithGenerics ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (KeyValuePair<,>), typeof (NullMixin)))
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
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The MixinConfiguration contains a ClassContext for the generic type "
        + ".*UninstantiableGeneric\\`1\\[T\\], of which it cannot make a closed type. "
            + "Because closed types are needed for validation, the MixinConfiguration cannot be validated as a whole. The configuration might still "
                + "be correct, but validation must be deferred to TypeFactory.GetActiveConfiguration.", MatchType = MessageMatch.Regex)]
    public void ValidationThrowsWhenGenericsCannotBeSpecialized ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (UninstantiableGeneric<>), typeof (NullMixin)))
        {
          MixinConfiguration.ActiveConfiguration.Validate ();
        }
      }
    }
  }
}