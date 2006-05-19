using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class ThreadUserProviderTest
  {
    // types

    // static members

    // member fields

    private ThreadUserProvider _provider;

    // construction and disposing

    public ThreadUserProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _provider = new ThreadUserProvider ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IUserProvider), _provider);
    }

    [Test]
    public void GetUser ()
    {
      Assert.AreSame (Thread.CurrentPrincipal, _provider.GetUser ());
    }
  }
}