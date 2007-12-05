using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextTests
{
  [TestFixture]
  public class ApplicationContextValidationTests
  {
    [Test]
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
          Assert.IsTrue (log.GetNumberOfSuccesses () > 0);
          Assert.AreEqual (0, log.GetNumberOfFailures ());
        }
      }
    }

    [Test]
    public void ValidateWithErrors ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (int), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    [Test]
    public void ValidateWithGenerics ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (KeyValuePair<,>), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    class UninstantiableGeneric<T>
        where T : ISerializable, IServiceProvider
    {
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The ApplicationContext contains a ClassContext for the generic type "
        + ".*UninstantiableGeneric\\`1\\[T\\], of which it cannot make a closed type. "
            + "Because closed types are needed for validation, the ApplicationContext cannot be validated as a whole. The configuration might still "
                + "be correct, but validation must be deferred to TypeFactory.GetActiveConfiguration.", MatchType = MessageMatch.Regex)]
    public void ValidationThrowsWhenGenericsCannotBeSpecialized ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (UninstantiableGeneric<>), typeof (NullMixin)))
        {
          MixinConfiguration.ActiveContext.Validate ();
        }
      }
    }
  }
}