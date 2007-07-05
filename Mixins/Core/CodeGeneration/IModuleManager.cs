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

    void SaveAssembly (string path);

    void InitializeInstance (IMixinTarget instance);
    void InitializeInstance (IMixinTarget instance, object[] extensions);
    void InitializeInstanceWithMixins (IMixinTarget instance, object[] mixinInstances);
    void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget targetInstance);
  }
}
