using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Reflection.Emit;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public abstract class ConditionExpression : Expression
  {
    public abstract OpCode BranchIfTrue { get; }
    public abstract OpCode BranchIfFalse { get; }
  }
}
