using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  // Converts an expression to a reference by saving it as a temporary local variable at time of emitting
  public class ExpressionReference : Reference
  {
    private Expression _expression;
    private MethodEmitter _methodEmitter;
    private Type _referenceType;

    public ExpressionReference (Type referenceType, Expression expression, MethodEmitter methodEmitter) : base (null)
    {
      _referenceType = referenceType;
      _expression = expression;
      _methodEmitter = methodEmitter;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      throw new NotImplementedException();
    }

    public override void LoadReference (ILGenerator gen)
    {
      LocalReference local = _methodEmitter.CodeBuilder.DeclareLocal (_referenceType);
      local.Generate (gen);
      new AssignStatement (local, _expression).Emit (_methodEmitter, gen);
      local.LoadReference (gen);
    }

    public override void StoreReference (ILGenerator gen)
    {
      throw new NotImplementedException();
    }
  }
}
