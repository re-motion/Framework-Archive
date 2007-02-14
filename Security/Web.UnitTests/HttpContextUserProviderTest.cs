using System;
using NUnit.Framework;

namespace Rubicon.Security.Web.UnitTests
{
  [TestFixture]
  public class HttpContextUserProviderTest
  {
    [Test]
    public void GetIsNull()
    {
      IUserProvider _userProvider = new HttpContextUserProvider();
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}