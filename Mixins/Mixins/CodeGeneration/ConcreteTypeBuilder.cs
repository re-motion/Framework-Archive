using System;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public class ConcreteTypeBuilder
  {
    public static readonly CallContextSingleton<IModuleManager> Scope = new CallContextSingleton<IModuleManager> (
        "Mixins.CodeGeneration.ConcreteTypeBuilder.Scope", delegate { return new DynamicProxy.ModuleManager(); });
    public static readonly CallContextSingleton<ConcreteTypeBuilder> Instance = new CallContextSingleton<ConcreteTypeBuilder> (
        "Mixins.CodeGeneration.ConcreteTypeBuilder.Instance", delegate { return new ConcreteTypeBuilder (); });

    // TODO: Add caching to this class
    public Type GetConcreteType (BaseClassDefinition configuration)
    {
      ITypeGenerator generator = Scope.Current.CreateTypeGenerator (configuration);
      Type finishedType = generator.GetBuiltType().CreateType();
      generator.InitializeStaticFields (finishedType);
      return finishedType;
    }
  }
}
