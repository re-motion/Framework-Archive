using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions
{
  public class MixinDefinition : ClassDefinition
  {
    private BaseClassDefinition _baseClass;
    private Dictionary<Type, InterfaceIntroductionDefinition> _interfaceIntroductions = new Dictionary<Type, InterfaceIntroductionDefinition> ();
    private Dictionary<MethodInfo, MethodDefinition> _initializationMethods = new Dictionary<MethodInfo, MethodDefinition> ();

    public MixinDefinition (Type type, BaseClassDefinition baseClass)
        : base (type)
    {
      if (type.IsInterface)
      {
        string message = string.Format("Interfaces ({0}) are not allowed as mixin types.", type.FullName);
        throw new ArgumentException (message, "type");
      }
      _baseClass = baseClass;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public IEnumerable<InterfaceIntroductionDefinition> InterfaceIntroductions
    {
      get { return _interfaceIntroductions.Values; }
    }

    public bool HasInterfaceIntroduction (Type type)
    {
      return _interfaceIntroductions.ContainsKey (type);
    }

    public void AddInterfaceIntroduction (InterfaceIntroductionDefinition introduction)
    {
      if (HasInterfaceIntroduction (introduction.Type))
      {
        string message = string.Format("Mixin {0} already has introduction {1}.", FullName, introduction.FullName);
        throw new InvalidOperationException (message);
      }
      _interfaceIntroductions.Add (introduction.Type, introduction);
    }

    public InterfaceIntroductionDefinition GetInterfaceIntroduction (Type type)
    {
      return HasInterfaceIntroduction(type) ? _interfaceIntroductions[type] : null;
    }

    public IEnumerable<MemberDefinition> Overrides
    {
      get
      {
        foreach (MemberDefinition member in Members)
        {
          if (member.Base != null)
          {
            yield return member;
          }
        }
      }
    }

    public IEnumerable<MethodDefinition> InitializationMethods
    {
      get { return _initializationMethods.Values; }
    }

    public bool HasInitializationMethod (MethodInfo method)
    {
      return _initializationMethods.ContainsKey (method);
    }

    public void AddInitializationMethod (MethodDefinition initializationMethod)
    {
      if (HasInitializationMethod (initializationMethod.MethodInfo))
      {
        string message = string.Format ("Cannot at initialization method {0} to mixin {1}, an equivalent initialization method ({2}) already exists.",
          initializationMethod.FullName, FullName, GetInitializationMethod (initializationMethod.MethodInfo).FullName);
        throw new InvalidOperationException (message);
      }
      _initializationMethods.Add (initializationMethod.MethodInfo, initializationMethod);
    }

    public MethodDefinition GetInitializationMethod (MethodInfo method)
    {
      return HasInitializationMethod (method) ? _initializationMethods[method] : null;
    }
  }
}
