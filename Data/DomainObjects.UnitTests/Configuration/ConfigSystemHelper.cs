using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  public class ConfigSystemHelper
  {
    private Enum _notStarted;
    private Enum _usable;
    private FakeInternalConfigSystem _fakeConfigSystem;

    public void SetUpConfigSystem ()
    {
      Type initStateType = typeof (ConfigurationElement).Assembly.GetType ("System.Configuration.ConfigurationManager+InitState", true, false);
      _notStarted = (Enum) Enum.Parse (initStateType, "NotStarted");
      _usable = (Enum) Enum.Parse (initStateType, "Usable");

      _fakeConfigSystem = new FakeInternalConfigSystem();
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", _fakeConfigSystem);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _usable);
    }

    public void SetUpConnectionString (string name, string connectionString, string providerName)
    {
      ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings (name, connectionString, providerName);
      ConnectionStringsSection connectionStringsSection = new ConnectionStringsSection ();
      connectionStringsSection.ConnectionStrings.Add (connectionStringSettings);

      _fakeConfigSystem.AddSection ("connectionStrings", connectionStringsSection);
    }

    public void TearDownConfigSystem ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _notStarted);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", null);
    }
  }
}