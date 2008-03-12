using System;
using System.Reflection;
using Rubicon.Mixins.Context.FluentBuilders;

namespace Rubicon.Mixins.Context.DeclarativeAnalyzers
{
  public class MixAnalyzer : RelationAnalyzerBase
  {
    public MixAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Assembly assembly)
    {
      foreach (MixAttribute attribute in assembly.GetCustomAttributes (typeof (MixAttribute), false))
        AnalyzeMixAttribute (attribute);
    }

    public virtual void AnalyzeMixAttribute (MixAttribute mixAttribute)
    {
      AddMixinAndAdjustException (mixAttribute.TargetType, mixAttribute.MixinType, mixAttribute.AdditionalDependencies, mixAttribute.SuppressedMixins);
    }
  }
}