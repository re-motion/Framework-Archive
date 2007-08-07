using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.CodeGeneration
{
  public interface IModuleManager
  {
    ITypeGenerator CreateTypeGenerator (TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider);

    string SignedAssemblyName { get; set; }
    string UnsignedAssemblyName { get; set; }

    string SignedModulePath { get; set; }
    string UnsignedModulePath { get; set; }

    bool HasAssemblies { get; }
    bool HasSignedAssembly { get; }
    bool HasUnsignedAssembly { get; }

    string[] SaveAssemblies ();

    void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances);
  }
}
