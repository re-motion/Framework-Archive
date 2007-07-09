using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.CodeGeneration
{
  public interface IModuleManager
  {
    ITypeGenerator CreateTypeGenerator (BaseClassDefinition configuration);
    IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition configuration);

    string AssemblyName { get; set; }
    string ModulePath { get; set; }

    bool HasAssembly { get; }
    string SaveAssembly ();

    void InitializeMixinTarget (IMixinTarget instance);
    void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances);
  }
}
