using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context
{
  internal class DeclarativeConfigurationAnalyzer
  {
    private readonly MixinConfiguration _parentConfiguration;
    private readonly Set<Type> _extenders;
    private readonly Set<Type> _users;
    private readonly Set<Type> _completeInterfaces;

    private MixinConfigurationBuilder _configurationBuilder;

    public DeclarativeConfigurationAnalyzer (MixinConfiguration parentConfiguration, Set<Type> extenders, Set<Type> users, Set<Type> completeInterfaces)
    {
      _parentConfiguration = parentConfiguration;
      _extenders = extenders;
      _completeInterfaces = completeInterfaces;
      _users = users;

      Analyze ();
    }

    public MixinConfiguration GetAnalyzedConfiguration
    {
      get { return _configurationBuilder.BuildConfiguration(); }
    }

    public void Analyze ()
    {
      _configurationBuilder = new MixinConfigurationBuilder (_parentConfiguration);
      foreach (Type extender in _extenders)
        AnalyzeExtender (extender);
      foreach (Type user in _users)
        AnalyzeUser (user);
      foreach (Type completeInterface in _completeInterfaces)
        AnalyzeCompleteInterface (completeInterface);
    }

    private void AnalyzeExtender (Type extender)
    {
      foreach (ExtendsAttribute mixinAttribute in extender.GetCustomAttributes (typeof (ExtendsAttribute), false))
      {
        Type mixinType = extender;
        if (mixinAttribute.MixinTypeArguments.Length > 0)
        {
          try
          {
            mixinType = mixinType.MakeGenericType (mixinAttribute.MixinTypeArguments);
          }
          catch (Exception ex)
          {
            string message = string.Format ("The ExtendsAttribute for target class {0} applied to mixin type {1} specified invalid generic type "
              + "arguments.", mixinAttribute.TargetType.FullName, extender.FullName);
            throw new ConfigurationException (message, ex);
          }
        }
        AddMixinToClass (mixinAttribute.TargetType, mixinType, mixinAttribute.AdditionalDependencies, mixinAttribute.SuppressedMixins);
      }
    }

    private void AnalyzeUser (Type user)
    {
      foreach (UsesAttribute usesAttribute in user.GetCustomAttributes (typeof (UsesAttribute), false))
        AddMixinToClass (user, usesAttribute.MixinType, usesAttribute.AdditionalDependencies, usesAttribute.SuppressedMixins);
    }

    private void AddMixinToClass (Type targetType, Type mixinType, IEnumerable<Type> explicitDependencies, IEnumerable<Type> suppressedMixins)
    {
      MixinContextBuilder mixinContextBuilder;
      try
      {
        mixinContextBuilder = _configurationBuilder.ForClass (targetType).AddMixin (mixinType);
      }
      catch (ArgumentException ex)
      {
        Type typeForMessage = mixinType;
        if (typeForMessage.IsGenericType)
          typeForMessage = typeForMessage.GetGenericTypeDefinition();
        string message = string.Format (
            "Two instances of mixin {0} are configured for target type {1}.",
            typeForMessage.FullName,
            targetType.FullName);
        throw new ConfigurationException (message, ex);
      }

      foreach (Type suppressedMixinType in suppressedMixins)
      {
        if (Rubicon.Utilities.ReflectionUtility.CanAscribe (mixinType, suppressedMixinType))
        {
          string message = string.Format ("Mixin type {0} applied to target class {1} suppresses itself.", mixinType.FullName,
              targetType.FullName);
          throw new ConfigurationException (message);
        }
      }

      mixinContextBuilder.WithDependencies (EnumerableUtility.ToArray (explicitDependencies))
          .SuppressMixins (EnumerableUtility.ToArray (suppressedMixins));
    }

    private void AnalyzeCompleteInterface (Type completeInterfaceType)
    {
      foreach (CompleteInterfaceAttribute ifaceAttribute in completeInterfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
        _configurationBuilder.ForClass (ifaceAttribute.TargetType).AddCompleteInterface (completeInterfaceType);
    }
  }
}