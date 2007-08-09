using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.CodeGeneration.DPExtensions
{
  public class CustomAttributeExpression : Expression
  {
    private static readonly MethodInfo s_getCustomAttributesMethod =
        typeof (Type).GetMethod ("GetCustomAttributes", new Type[] {typeof (Type), typeof (bool)}, null);

    private readonly Reference _attributeOwner;
    private readonly Type _attributeType;
    private readonly int _index;
    private readonly bool _inherited;
    private readonly Expression _getAttributeExpression;

    public CustomAttributeExpression (Reference attributeOwner, Type attributeType, int index, bool inherited)
    {
      _attributeOwner = attributeOwner;
      _attributeType = attributeType;
      _index = index;
      _inherited = inherited;

      Expression getAttributesExpression = new CastClassExpression (
          _attributeType.MakeArrayType (),
          new VirtualMethodInvocationExpression (
              _attributeOwner,
              s_getCustomAttributesMethod,
              new TypeTokenExpression (_attributeType),
              new ConstReference (_inherited).ToExpression ()));
      _getAttributeExpression =
          new LoadArrayExpressionElementExpression (getAttributesExpression, new ConstReference (_index).ToExpression (), _attributeType);
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _getAttributeExpression.Emit (member, gen);
    }
  }
}