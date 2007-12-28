using System;
using System.Collections.Generic;
using Rubicon.Mixins;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Provides a fluent interface for building <see cref="MixinConfiguration"/> objects.
  /// </summary>
  public class MixinConfigurationBuilder
  {
    private readonly MixinConfiguration _parentConfiguration;
    private readonly Dictionary<Type, ClassContextBuilder> _classContextBuilders = new Dictionary<Type, ClassContextBuilder>();

    public MixinConfigurationBuilder (MixinConfiguration parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    /// <summary>
    /// Gets the parent configuration used as a base for the newly built mixin configuration.
    /// </summary>
    /// <value>The parent configuration.</value>
    public virtual MixinConfiguration ParentConfiguration
    {
      get { return _parentConfiguration; }
    }

    /// <summary>
    /// Gets the class context builders collected so far via the fluent interfaces.
    /// </summary>
    /// <value>The class context builders collected so far.</value>
    public IEnumerable<ClassContextBuilder> ClassContextBuilders
    {
      get { return _classContextBuilders.Values; }
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      if (!_classContextBuilders.ContainsKey (targetType))
      {
        ClassContext parentContext = ParentConfiguration != null ? ParentConfiguration.GetClassContextNonRecursive (targetType) : null;
        ClassContextBuilder builder = new ClassContextBuilder (this, targetType, parentContext);
        _classContextBuilders.Add (targetType, builder);
      }
      return _classContextBuilders[targetType];
    }

    /// <summary>
    /// Begins configuration of a target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return ForClass (typeof (TTargetType));
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      IEnumerable<ClassContext> parentContexts = ParentConfiguration != null ? ParentConfiguration.ClassContexts : new ClassContext[0];
      MixinConfiguration builtConfiguration = new MixinConfiguration (ParentConfiguration);
      InheritanceAwareMixinConfigurationBuilder builder = new InheritanceAwareMixinConfigurationBuilder (builtConfiguration, parentContexts, ClassContextBuilders);
      return builder.BuildMixinConfiguration();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      MixinConfiguration configuration = BuildConfiguration();
      return configuration.EnterScope();
    }
  }
}