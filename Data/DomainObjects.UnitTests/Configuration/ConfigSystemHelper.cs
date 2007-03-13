using System;
using System.Configuration;
using System.Configuration.Internal;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  public class ConfigSystemHelper
  {
    private MockRepository _mocks;
    private Enum _notStarted;
    private Enum _usable;
    private IInternalConfigSystem _mockConfigSystem;

    public void SetUpConfigSystem ()
    {
      Type initStateType = typeof (ConfigurationElement).Assembly.GetType ("System.Configuration.ConfigurationManager+InitState", true, false);
      _notStarted = (Enum) Enum.Parse (initStateType, "NotStarted");
      _usable = (Enum) Enum.Parse (initStateType, "Usable");

      _mocks = new MockRepository ();
      _mockConfigSystem = _mocks.CreateMock<IInternalConfigSystem> ();
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", _mockConfigSystem);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _usable);
    }

    public void SetUpConnectionString (string name, string connectionString, string providerName)
    {
      ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings (name, connectionString, providerName);
      ConnectionStringsSection connectionStringsSection = new ConnectionStringsSection ();
      connectionStringsSection.ConnectionStrings.Add (connectionStringSettings);

      SetupResult.For (_mockConfigSystem.GetSection ("connectionStrings")).Return (connectionStringsSection);
    }

    public void TearDownConfigSystem ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _notStarted);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", null);
    }

    public void ReplayConfigSystem ()
    {
      _mocks.ReplayAll();
    }

    public void VerifyConfigSystem ()
    {
      _mocks.VerifyAll();
    }
  }
}