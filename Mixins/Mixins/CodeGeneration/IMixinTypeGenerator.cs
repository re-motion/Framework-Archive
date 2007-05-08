using System.Reflection.Emit;

namespace Mixins.CodeGeneration
{
  public interface IMixinTypeGenerator
  {
    TypeBuilder GetBuiltType ();
  }
}