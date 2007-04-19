using System;
using System.Reflection.Emit;

namespace Mixins.CodeGeneration
{
  public interface ITypeGenerator
  {
    TypeBuilder GetBuiltType ();

    void InitializeStaticFields (Type finishedType);
  }
}
