using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (ObjectFactory.Create<SimpleBusinessObjectClass> ().With (), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void SerializeAndDeserialize ()
    {
    }
  }
}