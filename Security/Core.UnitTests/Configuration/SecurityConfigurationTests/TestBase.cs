using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Security.Configuration;

namespace Rubicon.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  public class TestBase
  {
    private SecurityConfiguration _configuration;

    [SetUp]
    public virtual void SetUp ()
    {
      _configuration = new SecurityConfiguration();
      SetCurrentSecurityConfiguration (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      SetCurrentSecurityConfiguration (null);
    }

    protected SecurityConfiguration Configuration
    {
      get
      {
        return _configuration;
      }
    }

    private void SetCurrentSecurityConfiguration (SecurityConfiguration configuration)
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", configuration);
    }
  }
}