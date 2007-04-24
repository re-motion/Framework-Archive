using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Internal;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting.Configuration
{
  /// <summary>
  /// The <see cref="ConfigSystemHelper"/> class is intended to inject a fake implementation of the <see cref="IInternalConfigSystem"/> interface
  /// into the <see cref="ConfigurationManager"/> class. The feature can be used to override the config system with custom settings during unit tests.
  /// </summary>
  /// <remarks>
  /// The <see cref="ConfigSystemHelper"/> should only be used in cases where it is necesarry to inject configuration settings into third party code.
  /// The preferred solution is to use the <see cref="ConfigurationWrapper"/>'s well-known instance to access the configuration, thus allowing for 
  /// easy faking of the configuration during unit tests through changing the well-known instance to a test specific configuration object.
  /// </remarks>
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