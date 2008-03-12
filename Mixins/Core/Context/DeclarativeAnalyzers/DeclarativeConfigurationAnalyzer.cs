using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Mixins.Context.DeclarativeAnalyzers
{
  public class DeclarativeConfigurationAnalyzer
  {
    private readonly ExtendsAnalyzer _extendsAnalyzer;
    private readonly UsesAnalyzer _usesAnalyzer;
    private readonly MixAnalyzer _mixAnalyzer;
    private readonly CompleteInterfaceAnalyzer _completeInterfaceAnalyzer;

    public DeclarativeConfigurationAnalyzer (ExtendsAnalyzer extendsAnalyzer, UsesAnalyzer usesAnalyzer, CompleteInterfaceAnalyzer interfaceAnalyzer, 
        MixAnalyzer mixAnalyzer)
    {
      ArgumentUtility.CheckNotNull ("extendsAnalyzer", extendsAnalyzer);
      ArgumentUtility.CheckNotNull ("usesAnalyzer", usesAnalyzer);
      ArgumentUtility.CheckNotNull ("interfaceAnalyzer", interfaceAnalyzer);
      ArgumentUtility.CheckNotNull ("mixAnalyzer", mixAnalyzer);

      _extendsAnalyzer = extendsAnalyzer;
      _usesAnalyzer = usesAnalyzer;
      _completeInterfaceAnalyzer = interfaceAnalyzer;
      _mixAnalyzer = mixAnalyzer;
    }
    
    public void Analyze (IEnumerable<Type> types)
    {
      Set<Assembly> assemblies = new Set<Assembly>();

      foreach (Type type in types)
      {
        _extendsAnalyzer.Analyze (type);
        _usesAnalyzer.Analyze (type);
        _completeInterfaceAnalyzer.Analyze (type);

        if (!assemblies.Contains (type.Assembly))
        {
          assemblies.Add (type.Assembly);
          _mixAnalyzer.Analyze (type.Assembly);
        }
      }
    }
  }
}