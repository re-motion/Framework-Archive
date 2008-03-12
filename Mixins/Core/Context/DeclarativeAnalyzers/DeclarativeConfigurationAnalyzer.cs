using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.DeclarativeAnalyzers
{
  public class DeclarativeConfigurationAnalyzer
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;
    private readonly ExtendsAnalyzer _extendsAnalyzer;
    private readonly UsesAnalyzer _usesAnalyzer;
    private readonly CompleteInterfaceAnalyzer _completeInterfaceAnalyzer;

    public DeclarativeConfigurationAnalyzer (MixinConfigurationBuilder configurationBuilder, ExtendsAnalyzer extendsAnalyzer,
        UsesAnalyzer usesAnalyzer, CompleteInterfaceAnalyzer interfaceAnalyzer)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("extendsAnalyzer", extendsAnalyzer);
      ArgumentUtility.CheckNotNull ("usesAnalyzer", usesAnalyzer);
      ArgumentUtility.CheckNotNull ("interfaceAnalyzer", interfaceAnalyzer);

      _configurationBuilder = configurationBuilder;
      _extendsAnalyzer = extendsAnalyzer;
      _usesAnalyzer = usesAnalyzer;
      _completeInterfaceAnalyzer = interfaceAnalyzer;
    }
    
    public void Analyze (IEnumerable<Type> types)
    {
      foreach (Type extender in types)
        _extendsAnalyzer.Analyze (extender);

      foreach (Type user in types)
        _usesAnalyzer.Analyze (user);

      foreach (Type completeInterface in types)
        _completeInterfaceAnalyzer.Analyze (completeInterface);
    }
  }
}