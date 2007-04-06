using System;

namespace Mixins.Definitions
{
  public abstract class DependencyDefinitionBase<TRequirement, TSelf> : IVisitableDefinition
      where TRequirement : RequirementDefinitionBase<TRequirement, TSelf>
      where TSelf : DependencyDefinitionBase<TRequirement, TSelf>
  {
    public readonly DefinitionItemCollection<Type, TSelf> AggregatedDependencies =
        new DefinitionItemCollection<Type, TSelf> (delegate (TSelf d) { return d.RequiredType.Type; });

    private TRequirement _requirement;
    private MixinDefinition _depender;
    private TSelf _aggregator;

    public DependencyDefinitionBase (TRequirement requirement, MixinDefinition depender, TSelf aggregator)
    {
      _requirement = requirement;
      _depender = depender;
      _aggregator = aggregator;
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
      get { return Aggregator == null ? Depender : (IVisitableDefinition) Aggregator; }
    }

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