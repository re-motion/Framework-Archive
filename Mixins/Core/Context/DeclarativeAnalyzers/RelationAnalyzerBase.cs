using System;
using System.Collections.Generic;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.DeclarativeAnalyzers
{
  public abstract class RelationAnalyzerBase
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public RelationAnalyzerBase (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    protected void AddMixinAndAdjustException (Type targetType, Type mixinType, IEnumerable<Type> additionalDependencies, IEnumerable<Type> suppressedMixins)
    {
      try
      {
        _configurationBuilder.AddMixinToClass (targetType, mixinType, additionalDependencies, suppressedMixins);
      }
      catch (Exception ex)
      {
        throw new ConfigurationException (ex.Message, ex);
      }
    }
  }
}