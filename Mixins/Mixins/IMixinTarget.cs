using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins
{
  public interface IMixinTarget
  {
    BaseClassDefinition Configuration { get; }

    object[] Mixins { get; }
  }
}
