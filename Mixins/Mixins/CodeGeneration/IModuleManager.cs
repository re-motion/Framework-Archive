using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public interface IModuleManager
  {
    ITypeGenerator CreateTypeGenerator (BaseClassDefinition configuration);
    IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition configuration);

    string SaveAssembly ();

    void InitializeInstance (IMixinTarget instance);
    void InitializeInstance (IMixinTarget instance, object[] extensions);
    void InitializeInstanceWithMixins (IMixinTarget instance, object[] mixinInstances);
    void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget targetInstance);
  }
}
