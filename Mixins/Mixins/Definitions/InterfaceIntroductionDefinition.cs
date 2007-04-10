using System;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    private Type _type;
    private MixinDefinition _implementer;

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      _type = type;
      _implementer = implementer;
    }

    public Type Type
    {
      get { return _type; }
    }

    public MixinDefinition Implementer
    {
      get { return _implementer; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public BaseClassDefinition BaseClass
    {
      get { return Implementer.BaseClass; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
