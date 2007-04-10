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
  }
}
