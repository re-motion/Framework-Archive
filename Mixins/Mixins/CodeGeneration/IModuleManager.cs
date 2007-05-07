using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public interface IModuleManager
  {
    ITypeGenerator CreateTypeGenerator (BaseClassDefinition configuration);
    string SaveAssembly ();

    void InitializeInstance (object instance);
    void InitializeInstance (object instance, object[] extensions, object firstBaseCallProxy);

    void InitializeInstanceWithMixins (object instance, object[] mixinInstances);
  }
}
