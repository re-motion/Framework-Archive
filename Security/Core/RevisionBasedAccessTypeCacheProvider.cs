using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class RevisionBasedAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    // constants

    // types

    // static members

    // member fields

    private Cache<Tuple<SecurityContext, string>, AccessType[]> _cache = new Cache<Tuple<SecurityContext, string>, AccessType[]> ();
    private int _revision = 0;
    private object _syncRoot = new object ();

    // construction and disposing

    public RevisionBasedAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ()
    {
      ISecurityService securityService = SecurityConfiguration.Current.SecurityService;
      if (securityService == null)
        throw new SecurityConfigurationException ("The security service has not been configured.");

      int currentRevision = securityService.GetRevision ();
      if (_revision < currentRevision)
      {
        lock (_syncRoot)
        {
          if (_revision < currentRevision)
          {
            _revision = currentRevision;
            _cache = new Cache<Tuple<SecurityContext, string>, AccessType[]> ();
          }
        }
      }

      return _cache;
    }
  }
}