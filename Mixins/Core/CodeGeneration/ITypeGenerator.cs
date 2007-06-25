using System;
using System.Reflection.Emit;

namespace Rubicon.Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    Type GetBuiltType ();
  }
}
