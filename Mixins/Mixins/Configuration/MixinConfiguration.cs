using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Configuration
{
  public class MixinConfiguration : ClassConfiguration
  {
    private BaseClassConfiguration _baseClass;
    private Dictionary<Type, InterfaceIntroductionConfiguration> _interfaceIntroductions = new Dictionary<Type, InterfaceIntroductionConfiguration> ();
    private Dictionary<MethodInfo, MethodConfiguration> _initializationMethods = new Dictionary<MethodInfo, MethodConfiguration> ();

    public MixinConfiguration (Type type, BaseClassConfiguration baseClass)
        : base (type)
    {
      if (type.IsInterface)
      {
        string message = string.Format("Interfaces ({0}) are not allowed as mixin types.", type.FullName);
        throw new ArgumentException (message, "type");
      }
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

    public IEnumerable<MethodConfiguration> InitializationMethods
    {
      get { return _initializationMethods.Values; }
    }

    public bool HasInitializationMethod (MethodInfo method)
    {
      return _initializationMethods.ContainsKey (method);
    }

    public void AddInitializationMethod (MethodConfiguration initializationMethod)
    {
      if (HasInitializationMethod (initializationMethod.MethodInfo))
      {
        string message = string.Format ("Cannot at initialization method {0} to mixin {1}, an equivalent initialization method ({2}) already exists.",
          initializationMethod.FullName, FullName, GetInitializationMethod (initializationMethod.MethodInfo).FullName);
        throw new InvalidOperationException (message);
      }
      _initializationMethods.Add (initializationMethod.MethodInfo, initializationMethod);
    }

    public MethodConfiguration GetInitializationMethod (MethodInfo method)
    {
      return HasInitializationMethod (method) ? _initializationMethods[method] : null;
    }
  }
}
