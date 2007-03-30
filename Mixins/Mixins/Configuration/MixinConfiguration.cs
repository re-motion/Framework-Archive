using System;
using System.Collections.Generic;

namespace Mixins.Configuration
{
  public class MixinConfiguration : ClassConfiguration
  {
    private BaseClassConfiguration _baseClass;
    private Dictionary<Type, InterfaceIntroductionConfiguration> _interfaceIntroductions = new Dictionary<Type, InterfaceIntroductionConfiguration> ();

    public MixinConfiguration (Type type, BaseClassConfiguration baseClass)
        : base (type)
    {
      _baseClass = baseClass;
    }

    public BaseClassConfiguration BaseClass
    {
      get { return _baseClass; }
    }

    public IEnumerable<InterfaceIntroductionConfiguration> InterfaceIntroductions
    {
      get { return _interfaceIntroductions.Values; }
    }

    public bool HasInterfaceIntroduction (Type type)
    {
      return _interfaceIntroductions.ContainsKey (type);
    }

    public void AddInterfaceIntroduction (InterfaceIntroductionConfiguration introduction)
    {
      if (HasInterfaceIntroduction (introduction.Type))
      {
        string message = string.Format("Mixin {0} already has introduction {1}.", FullName, introduction.FullName);
        throw new InvalidOperationException (message);
      }
      _interfaceIntroductions.Add (introduction.Type, introduction);
    }

    public InterfaceIntroductionConfiguration GetInterfaceIntroduction (Type type)
    {
      return HasInterfaceIntroduction(type) ? _interfaceIntroductions[type] : null;
    }

    public IEnumerable<MemberConfiguration> Overrides
    {
      get
      {
        foreach (MemberConfiguration member in Members)
        {
          if (member.Base != null)
          {
            yield return member;
          }
        }
      }
    }
  }
}
