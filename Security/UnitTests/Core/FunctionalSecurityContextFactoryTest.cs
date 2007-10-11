using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.Core.SampleDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Security.UnitTests.Core
{
  [TestFixture]
  public class FunctionalSecurityContextFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      ISecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));

      SecurityContext context = factory.CreateSecurityContext ();
      Assert.IsNotNull (context);
      Assert.AreEqual ("Rubicon.Security.UnitTests.Core.SampleDomain.SecurableObject, Rubicon.Security.UnitTests", context.Class);
    }

    [Test]
    public void Serialization ()
    {
      FunctionalSecurityContextFactory factory = new FunctionalSecurityContextFactory (typeof (SecurableObject));
      FunctionalSecurityContextFactory deserializedFactory = Serializer.SerializeAndDeserialize (factory);
      Assert.AreNotSame (factory, deserializedFactory);

      SecurityContext context1 = factory.CreateSecurityContext ();
      SecurityContext context2 = deserializedFactory.CreateSecurityContext ();
      Assert.AreNotSame (context1, context2);
      Assert.AreEqual (context1, context2);
    }
  }
}