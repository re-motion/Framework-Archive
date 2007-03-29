using System;

namespace Mixins.Configuration
{
  public class InterfaceIntroductionConfiguration
  {
    private Type _type;
    private MixinConfiguration _implementer;

    public InterfaceIntroductionConfiguration (Type type, MixinConfiguration implementer)
    {
      _type = type;
      _implementer = implementer;
    }

    public Type Type
    {
      get { return _type; }
    }

    public MixinConfiguration Implementer
    {
      get { return _implementer; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }
  }
}
