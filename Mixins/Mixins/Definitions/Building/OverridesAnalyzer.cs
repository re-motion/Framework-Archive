using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Rubicon;
using Rubicon.Collections;

namespace Mixins.Definitions.Building
{
  public class OverridesAnalyzer<T> where T : MemberDefinition
  {
    private Func<IEnumerable<T>> _baseMemberGetter;
    private MultiDictionary<string, T> _baseMembersByNameCache = null;

    public OverridesAnalyzer(Func<IEnumerable<T>> baseMemberGetter)
    {
      _baseMemberGetter = baseMemberGetter;
    }

    public IEnumerable<Tuple<T, T>> Analyze (IEnumerable<T> overriderMembers)
    {
      foreach (T member in overriderMembers)
      {
        if (member.MemberInfo.IsDefined (typeof (OverrideAttribute), true))
        {
          T baseMember;
          if (BaseMembersByName.ContainsKey (member.Name))
          {
            IEnumerable<T> candidates = BaseMembersByName[member.Name];
            baseMember = FindBaseMember (member, candidates);
          }
          else
            baseMember = null;

          if (baseMember == null)
          {
            string message = string.Format ("Could not find base member for overrider {0}.", member.FullName);
            throw new ConfigurationException (message);
          }
          yield return new Tuple<T, T> (member, baseMember);
        }
      }
    }

    private MultiDictionary<string, T> BaseMembersByName
    {
      get
      {
        EnsureMembersCached();
        return _baseMembersByNameCache;
      }
    }

    private void EnsureMembersCached ()
    {
      if (_baseMembersByNameCache == null)
      {
        _baseMembersByNameCache = new MultiDictionary<string, T> ();
        foreach (T member in _baseMemberGetter())
          _baseMembersByNameCache.Add (member.Name, member);
      }
    }

    private T FindBaseMember (T overrider, IEnumerable<T> candidates)
    {
      T result = null;
      foreach (T candidate in candidates)
      {
        if (candidate.Name == overrider.Name && candidate.CanBeOverriddenBy (overrider))
        {
          if (result != null)
          {
            string message = string.Format (
                "Ambiguous override: Member {0} has possible base members {1} and {2}.",
                overrider.FullName,
                result.FullName,
                candidate.FullName);
            throw new ConfigurationException (message);
          }
          else
            result = candidate;
        }
      }
      return result;
    }
  }
}
