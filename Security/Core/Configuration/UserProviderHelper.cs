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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IUserProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class UserProviderHelper : ProviderHelperBase<IUserProvider>
  {
    private const string c_threadUserProviderWellKnownName = "Thread";
    private const string c_httpContexUserProviderWellKnownName = "HttpContext";

    private readonly object _sync = new object();
    private Type _httpContextUserProviderType;

    public UserProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultUserProvider", c_threadUserProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("userProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_threadUserProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_httpContexUserProviderWellKnownName);

      if (DefaultProviderName.Equals (c_httpContexUserProviderWellKnownName, StringComparison.Ordinal))
        EnsureHttpContextUserProviderTypeInitialized();
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownThreadUserProvider (collection);
      EnsureWellKnownHttpContextUserProvider (collection);
    }

    private void EnsureWellKnownThreadUserProvider (ProviderCollection collection)
    {
      collection.Add (new ThreadUserProvider(c_threadUserProviderWellKnownName, new NameValueCollection()));
    }

    private void EnsureWellKnownHttpContextUserProvider (ProviderCollection collection)
    {
      if (_httpContextUserProviderType != null)
      {
       collection.Add ((ExtendedProviderBase) Activator.CreateInstance (
          _httpContextUserProviderType, 
          new object[] { c_httpContexUserProviderWellKnownName, new NameValueCollection()}));
      }
    }

    private void EnsureHttpContextUserProviderTypeInitialized ()
    {
      if (_httpContextUserProviderType == null)
      {
        lock (_sync)
        {
          if (_httpContextUserProviderType == null)
          {
            _httpContextUserProviderType = GetTypeWithMatchingVersionNumber (
                DefaultProviderNameProperty,
                "Remotion.Web.Security",
                "Remotion.Web.Security.HttpContextUserProvider");
          }
        }
      }
    }
  }
}
