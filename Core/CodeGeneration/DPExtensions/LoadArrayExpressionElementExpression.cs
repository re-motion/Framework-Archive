using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration.DPExtensions
{
  public class LoadArrayExpressionElementExpression : Expression
  {
    private readonly Expression _arrayExpression;
    private readonly Expression _elementIndexExpression;
    private readonly Type _elementType;

    public LoadArrayExpressionElementExpression (Expression arrayExpression, Expression elementIndexExpression, Type elementType)
    {
      ArgumentUtility.CheckNotNull ("arrayExpression", arrayExpression);
      ArgumentUtility.CheckNotNull ("elementIndexExpression", elementIndexExpression);
      ArgumentUtility.CheckNotNull ("elementType", elementType);

      _arrayExpression = arrayExpression;
      _elementIndexExpression = elementIndexExpression;
      _elementType = elementType;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _arrayExpression.Emit (member, gen);
      _elementIndexExpression.Emit (member, gen);
      gen.Emit (OpCodes.Ldelem, _elementType);
    }
  }
}