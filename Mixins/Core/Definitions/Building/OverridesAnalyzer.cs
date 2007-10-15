using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class OverridesAnalyzer<T> where T : MemberDefinition
  {
    private readonly Type _attributeType = null;
    private readonly IEnumerable<T> _baseMembers;

    private MultiDictionary<string, T> _baseMembersByNameCache = null;

    public OverridesAnalyzer (Type attributeType, IEnumerable<T> baseMembers)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);
      ArgumentUtility.CheckNotNull ("baseMembers", baseMembers);

      _attributeType = attributeType;
      _baseMembers = baseMembers;
    }

    public IEnumerable<Tuple<T, T>> Analyze (IEnumerable<T> overriderMembers)
    {
      ArgumentUtility.CheckNotNull ("overriderMembers", overriderMembers);

      foreach (T member in overriderMembers)
      {
        if (AttributeUtility.IsDefined(member.MemberInfo, _attributeType, true))
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
        foreach (T member in _baseMembers)
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
