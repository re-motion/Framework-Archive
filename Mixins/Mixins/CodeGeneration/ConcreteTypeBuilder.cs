using System;
using System.Reflection.Emit;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder
  {
    private IModuleManager _moduleManager;

    public ConcreteTypeBuilder(IModuleManager moduleManager)
    {
      _moduleManager = moduleManager;
    }

    public TypeBuilder BuildConcreteType (BaseClassDefinition configuration)
    {
      ITypeGenerator generator = _moduleManager.CreateTypeGenerator (configuration);
      return generator.GetTypeBuilder ();
    }

    public string SaveScopeToFile ()
    {
      return _moduleManager.SaveAssembly ();
    }
  }
}
