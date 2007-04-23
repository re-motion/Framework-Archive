using System;
using System.Collections.Generic;
using Rubicon.Collections;

namespace Mixins.Definitions.Building
{
  public class OverridesAnalyzer
  {
    public IEnumerable<Tuple<T, T>> Analyze<T> (IEnumerable<T> mixinMembers, IEnumerable<T> classMembers)
        where T : MemberDefinition
    {
      foreach (T member in mixinMembers)
      {
        if (member.MemberInfo.IsDefined (typeof (OverrideAttribute), true))
        {
          T baseMember = FindBaseMember (member, classMembers);
          if (baseMember == null)
          {
            string message = string.Format ("Could not find base member for overrider {0}.", member.FullName);
            throw new ConfigurationException (message);
          }
          yield return new Tuple<T, T> (member, baseMember);
        }
      }
    }

    private T FindBaseMember<T> (T overrider, IEnumerable<T> classMembers)
        where T : MemberDefinition
    {
      foreach (T classMember in classMembers)
      {
        if (classMember.Name == overrider.Name && classMember.CanBeOverriddenBy (overrider))
        {
          return classMember;
        }
      }
      return null;
    }
  }
}
