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

    // TODO: Add caching
    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ITypeGenerator generator = _moduleManager.CreateTypeGenerator (configuration);
      Type finishedType = generator.GetBuiltType().CreateType();
      generator.InitializeStaticFields (finishedType);
      return finishedType;
    }

    public string SaveScopeToFile ()
    {
      return _moduleManager.SaveAssembly ();
    }
  }
}
