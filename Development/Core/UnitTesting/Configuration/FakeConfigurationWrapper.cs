/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that fakes the access to the configuration system. Use this class for setting up
  /// unit tests.
  /// </summary>
  public sealed class FakeConfigurationWrapper : ConfigurationWrapper
  {
    private Dictionary<string, object> _sections = new Dictionary<string, object> ();
    private ConnectionStringsSection _connectionStringsSection = new ConnectionStringsSection();
    private NameValueCollection _appSettings = new NameValueCollection();

    public FakeConfigurationWrapper()
    {
    }

    public void SetUpSection (string configKey, object section)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configKey", configKey);
      ArgumentUtility.CheckNotNull ("section", section);

      _sections.Add (configKey, section);
    }

    public void SetUpConnectionString (string name, string connectionString, string providerName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);

      _connectionStringsSection.ConnectionStrings.Add (new ConnectionStringSettings (name, connectionString, providerName));
    }

    public void SetUpAppSetting (string name, string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("value", value);

      _appSettings.Add (name, value);
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      object value;
      if (_sections.TryGetValue (sectionName, out value))
        return value;
      return null;
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _connectionStringsSection.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _appSettings[name];
    }
  }
}
