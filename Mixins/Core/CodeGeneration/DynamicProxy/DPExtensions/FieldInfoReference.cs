using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  internal class FieldInfoReference : Reference
  {
    private FieldInfo field;

    public FieldInfoReference (Reference owner, FieldInfo field)
        : base (owner)
    {
      this.field = field;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Ldflda, field);
    }

    public override void LoadReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Ldfld, field);
    }

    public override void StoreReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Stfld, field);
    }
  }
}