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

    void InitializeInstance (object instance);
    void InitializeInstance (object instance, object[] extensions);
    void InitializeInstanceWithMixins (object instance, object[] mixinInstances);
    void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, object targetInstance);
  }
}
