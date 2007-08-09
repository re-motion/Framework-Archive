using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.CodeGeneration.DPExtensions
{
  public class CastClassExpression : Expression
  {
    private readonly Type targetType;
    private readonly Expression sourceExpression;

    public CastClassExpression (Type targetType, Expression sourceExpression)
    {
      this.targetType = targetType;
      this.sourceExpression = sourceExpression;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      sourceExpression.Emit (member, gen);
      gen.Emit (OpCodes.Castclass, targetType);
    }
  }
}
