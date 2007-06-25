using System;
using System.Reflection.Emit;

namespace Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    Type GetBuiltType ();
  }
}
