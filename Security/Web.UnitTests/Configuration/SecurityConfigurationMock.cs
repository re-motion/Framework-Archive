using System;
using Rubicon.Security.Configuration;

namespace Rubicon.Security.Web.UnitTests.Configuration
{
  public class SecurityConfigurationMock : SecurityConfiguration
  {
    public new static void SetCurrent (SecurityConfiguration configuration)
    {
      SecurityConfiguration.SetCurrent (configuration);
    }

    public SecurityConfigurationMock()
    {
    }
  }
}