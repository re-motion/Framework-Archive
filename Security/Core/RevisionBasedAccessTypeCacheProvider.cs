using System;
using System.Configuration.Provider;
using System.Runtime.Remoting.Messaging;
using Rubicon.Collections;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class RevisionBasedAccessTypeCacheProvider : ProviderBase, IGlobalAccessTypeCacheProvider
  {
    // constants

    // types

    // static members

    private static string s_revisionKey = typeof (RevisionBasedAccessTypeCacheProvider).AssemblyQualifiedName + "_Revision";

    // member fields

    private InterlockedCache<Tuple<SecurityContext, string>, AccessType[]> _cache = new InterlockedCache<Tuple<SecurityContext, string>, AccessType[]> ();
    private int _revision = 0;
    private object _syncRoot = new object ();

    // construction and disposing

    public RevisionBasedAccessTypeCacheProvider ()
    {
    }

    // methods and properties

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ()
    {
      int currentRevision = GetCurrentRevision ();
      if (_revision < currentRevision)
      {
        lock (_syncRoot)
        {
          if (_revision < currentRevision)
          {
            _revision = currentRevision;
            _cache = new InterlockedCache<Tuple<SecurityContext, string>, AccessType[]> ();
          }
        }
      }

      return _cache;
    }

    private int GetCurrentRevision()
    {
      int? revision = (int?) CallContext.GetData (s_revisionKey);
      if (!revision.HasValue)
      {
        revision = SecurityConfiguration.Current.SecurityProvider.GetRevision ();
        CallContext.SetData (s_revisionKey, revision);
      }

      return revision.Value;
    }

    bool INullableObject.IsNull
    {
      get { return false; }
    }
  }
}