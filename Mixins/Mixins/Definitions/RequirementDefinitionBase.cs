using System;

namespace Mixins.Definitions
{
  public abstract class RequirementDefinitionBase: IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, MixinDefinition> Requirers =
        new DefinitionItemCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });

    private BaseClassDefinition _baseClass;
    private Type _type;

    public RequirementDefinitionBase(BaseClassDefinition baseClass, Type type)
    {
      _baseClass = baseClass;
      _type = type;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}