using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    Type GetBuiltType ();
    IEnumerable<Tuple<MixinDefinition, Type>> GetBuiltMixinTypes ();
  }
}
