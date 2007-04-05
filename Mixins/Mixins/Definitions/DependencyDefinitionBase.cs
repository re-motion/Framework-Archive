using System;

namespace Mixins.Definitions
{
  public abstract class DependencyDefinitionBase<TRequirement> : IVisitableDefinition
      where TRequirement : RequirementDefinitionBase
  {
    private TRequirement _requirement;
    private MixinDefinition _depender;

    public DependencyDefinitionBase (TRequirement requirement, MixinDefinition depender)
    {
      _requirement = requirement;
      _depender = depender;
    }

    public TRequirement RequiredType
    {
      get { return _requirement; }
    }

    public MixinDefinition Depender
    {
      get { return _depender; }
    }

    public string FullName
    {
      get { return RequiredType.FullName; }
    }

    public bool IsAggregateOnly
    {
      get { return RequiredType.Type.IsInterface && RequiredType.Type.GetMethods ().Length == 0; }
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