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

    private IUserProvider _userProvider;

    // construction and disposing

    public ThreadUserProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _userProvider = new ThreadUserProvider ();
    }

    [Test]
    public void GetUser ()
    {
      Assert.AreSame (Thread.CurrentPrincipal, _userProvider.GetUser ());
    }
    
    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}