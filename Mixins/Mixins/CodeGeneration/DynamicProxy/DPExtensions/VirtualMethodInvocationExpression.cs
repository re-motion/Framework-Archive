using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;
using System.Reflection.Emit;
using System.Reflection;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public class VirtualMethodInvocationExpression : MethodInvocationExpression
  {
    public VirtualMethodInvocationExpression(MethodInfo method, params Expression[] args)
			: base (method, args)
		{
		}

		public VirtualMethodInvocationExpression(MethodEmitter method, params Expression[] args)
		  : base (method, args)
		{
		}

		public VirtualMethodInvocationExpression(Reference owner, MethodEmitter method, params Expression[] args)
      : base (owner, method, args)
		{
		}

    public VirtualMethodInvocationExpression (Reference owner, MethodInfo method, params Expression[] args)
      : base (owner, method, args)
		{
		}

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      ArgumentsUtil.EmitLoadOwnerAndReference (owner, gen);

      foreach (Expression exp in args)
      {
        exp.Emit (member, gen);
      }

      gen.EmitCall (OpCodes.Callvirt, method, null);
    }
  }
}
