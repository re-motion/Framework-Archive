using System;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{RequiredType.Type}, Depender = {Depender.Type}")]
  public abstract class DependencyDefinitionBase<TRequirement, TSelf> : IVisitableDefinition
      where TRequirement : RequirementDefinitionBase<TRequirement, TSelf>
      where TSelf : DependencyDefinitionBase<TRequirement, TSelf>
  {
    public readonly DefinitionItemCollection<Type, TSelf> AggregatedDependencies;

    private TRequirement _requirement; // the required face or base interface
    private MixinDefinition _depender; // the mixin (directly or indirectly) defining the requirement
    private TSelf _aggregator; // the outer dependency containing this dependency, if defined indirectly

    public DependencyDefinitionBase (TRequirement requirement, MixinDefinition depender, TSelf aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("depender", depender);

      _requirement = requirement;
      _depender = depender;
      _aggregator = aggregator;

      AggregatedDependencies = new DefinitionItemCollection<Type, TSelf> (
          delegate (TSelf d) { return d.RequiredType.Type; },
          HasSameDependerAsAggregator);
    }

    public bool HasSameDependerAsAggregator (TSelf newAggregatedDependency)
    {
      ArgumentUtility.CheckNotNull ("newAggregatedDependency", newAggregatedDependency);
      return newAggregatedDependency.Depender == _depender;
    }

    public TRequirement RequiredType
    {
      get { return _requirement; }
    }

    public MixinDefinition Depender
    {
      get { return _depender; }
    }

    public TSelf Aggregator
    {
      get { return _aggregator; }
    }

    public string FullName
    {
      get { return RequiredType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get
      {
        if (Aggregator != null)
        {
          return Aggregator;
        }
        else
        {
          return Depender;
        }
      }
    }

    // aggregates hold nested dependencies
    public bool IsAggregate
    {
      get { return AggregatedDependencies.Count > 0; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);

    public ClassDefinition GetImplementer()
    {
      if (RequiredType.Type.IsAssignableFrom (_depender.BaseClass.Type))
      {
        return _depender.BaseClass;
      }
      else if (_depender.BaseClass.IntroducedInterfaces.HasItem (RequiredType.Type))
      {
        return _depender.BaseClass.IntroducedInterfaces[RequiredType.Type].Implementer;
      }
      else
      {
        return null;
      }
    }
  }
}