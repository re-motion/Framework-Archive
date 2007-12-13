using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  public class MixinConfigurationBuilder
  {
    private readonly MixinConfiguration _parentConfiguration;
    private readonly List<ClassContextBuilder> _classContextBuilders = new List<ClassContextBuilder> ();

    public MixinConfigurationBuilder (MixinConfiguration parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    public virtual MixinConfiguration ParentConfiguration
    {
      get { return _parentConfiguration; }
    }

    public IEnumerable<ClassContextBuilder> ClassContextBuilders
    {
      get { return _classContextBuilders; }
    }

    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ClassContext parentContext = ParentConfiguration != null ? ParentConfiguration.GetClassContext (targetType) : null;
      ClassContextBuilder builder = new ClassContextBuilder (this, targetType, parentContext);
      _classContextBuilders.Add (builder);
      return builder;
    }

    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return ForClass (typeof (TTargetType));
    }

    public virtual MixinConfiguration BuildConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfiguration (_parentConfiguration);
      foreach (ClassContextBuilder classContextBuilder in _classContextBuilders)
        classContextBuilder.BuildClassContext (configuration);
      return configuration;
    }

    public virtual IDisposable EnterScope ()
    {
      MixinConfiguration configuration = BuildConfiguration();
      return configuration.EnterScope();
    }
  }
}