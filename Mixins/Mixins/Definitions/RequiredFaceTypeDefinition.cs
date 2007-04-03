using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  public class RequiredBaseCallTypeDefinition : IVisitableDefinition
  {
    private BaseClassDefinition _baseClass;
    private Type _type;

    public RequiredBaseCallTypeDefinition (BaseClassDefinition baseClass, Type type)
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

    public void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
    }
  }
}
