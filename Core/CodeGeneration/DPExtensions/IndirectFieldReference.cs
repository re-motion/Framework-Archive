using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.CodeGeneration.DPExtensions
{
  public class IndirectFieldReference : Reference
  {
    private readonly FieldInfo _field;

    public IndirectFieldReference(Reference owner, FieldInfo field)
      : base (owner)
    {
      _field = field;
    }

    public FieldInfo Reference
    {
      get { return _field; }
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Ldflda, _field);
    }

    public override void LoadReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Ldfld, _field);
    }

    public override void StoreReference (ILGenerator gen)
    {
      gen.Emit (OpCodes.Stfld, _field);
    }
  }
}
