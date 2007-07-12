using System;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{RequiredType.Type}, Depender = {Depender.Type}")]
  public abstract class DependencyDefinitionBase : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<Type, DependencyDefinitionBase> AggregatedDependencies;

    private RequirementDefinitionBase _requirement; // the required face or base interface
    private MixinDefinition _depender; // the mixin (directly or indirectly) defining the requirement
    private DependencyDefinitionBase _aggregator; // the outer dependency containing this dependency, if defined indirectly

    public DependencyDefinitionBase (RequirementDefinitionBase requirement, MixinDefinition depender, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("depender", depender);
      ArgumentUtility.CheckType ("aggregator", aggregator, this.GetType ());

      _requirement = requirement;
      _depender = depender;
      _aggregator = aggregator;

      AggregatedDependencies = new UniqueDefinitionCollection<Type, DependencyDefinitionBase> (
          delegate (DependencyDefinitionBase d) { return d.RequiredType.Type; },
          HasSameDepender);
    }

    public bool HasSameDepender (DependencyDefinitionBase dependencyToCheck)
    {
      ArgumentUtility.CheckNotNull ("dependencyToCheck", dependencyToCheck);
      return dependencyToCheck.Depender == _depender;
    }

    public RequirementDefinitionBase RequiredType
    {
      get { return _requirement; }
    }

    public MixinDefinition Depender
    {
      get { return _depender; }
    }

    public DependencyDefinitionBase Aggregator
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

    public virtual ClassDefinitionBase GetImplementer()
    {
      if (RequiredType.Type.IsAssignableFrom (_depender.BaseClass.Type))
        return _depender.BaseClass;
      else if (_depender.BaseClass.IntroducedInterfaces.ContainsKey (RequiredType.Type))
        return _depender.BaseClass.IntroducedInterfaces[RequiredType.Type].Implementer;
      else
        return null;
    }
  }
}