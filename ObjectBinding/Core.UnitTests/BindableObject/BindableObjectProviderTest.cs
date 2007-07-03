using System;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest
  {
    [Test]
    [Ignore("TODO: test")]
    public void GetInstance ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetService ()
    {
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();

      BindableObjectClass outValue;
      Assert.That (provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleClass), out outValue), Is.False);

      BindableObjectClass actual = provider.GetBindableObjectClass (typeof (SimpleClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (provider));
      BindableObjectClass cachedBindableObjectClass;
      Assert.That (provider.BusinessObjectClassCache.TryGetValue (typeof (SimpleClass), out cachedBindableObjectClass), Is.True);
      Assert.That (actual, Is.SameAs (cachedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();

      Assert.That (provider.GetBindableObjectClass (typeof (SimpleClass)), Is.SameAs (provider.GetBindableObjectClass (typeof (SimpleClass))));
    }
  }
}