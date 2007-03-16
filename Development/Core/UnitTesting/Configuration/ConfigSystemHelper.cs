using System;
using System.Collections.Specialized;
using System.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting.Configuration
{
  public class ConfigSystemHelper
  {
    private Enum _notStarted;
    private Enum _usable;
    private FakeInternalConfigSystem _fakeConfigSystem;
    private ConnectionStringsSection _connectionStringsSection = new ConnectionStringsSection ();
    private NameValueCollection _appSettings = new NameValueCollection();

    public void SetUpConfigSystem()
    {
      Type initStateType = typeof (ConfigurationElement).Assembly.GetType ("System.Configuration.ConfigurationManager+InitState", true, false);
      _notStarted = (Enum) Enum.Parse (initStateType, "NotStarted");
      _usable = (Enum) Enum.Parse (initStateType, "Usable");

      _fakeConfigSystem = new FakeInternalConfigSystem();
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", _fakeConfigSystem);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _usable);

      _fakeConfigSystem.AddSection ("connectionStrings", _connectionStringsSection);
      _fakeConfigSystem.AddSection ("appSettings", _appSettings);
    }

    public void SetUpConnectionString (string name, string connectionString, string providerName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);
      
      _connectionStringsSection.ConnectionStrings.Add (new ConnectionStringSettings (name, connectionString, providerName));
    }

    public void SetUpAppSetting (string name, string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("name", name);

      _appSettings.Add (name, key);
    }
    
    public void TearDownConfigSystem()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_initState", _notStarted);
      PrivateInvoke.SetNonPublicStaticField (typeof (ConfigurationManager), "s_configSystem", null);
    }
  }
}