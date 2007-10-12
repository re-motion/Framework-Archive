using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.Mixins.Samples.DynamicMixinBuilding
{
  internal class PopStatement : Statement
  {
    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Pop);
    }
  }
}