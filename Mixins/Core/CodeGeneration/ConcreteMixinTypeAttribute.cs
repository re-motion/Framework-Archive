using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteMixinTypeAttribute : ConcreteMixedTypeAttribute
  {
    private static ConstructorInfo s_attributeCtor =
        typeof (ConcreteMixinTypeAttribute).GetConstructor (
            new Type[] {typeof (int), typeof (Type), typeof (Type[]), typeof (Type[]), typeof (Type[])});

    public static ConcreteMixinTypeAttribute FromClassContext (int mixinIndex, ClassContext baseClassContext)
    {
      ConcreteMixedTypeAttribute baseAttribute = ConcreteMixedTypeAttribute.FromClassContext (baseClassContext);
      return new ConcreteMixinTypeAttribute (mixinIndex, baseAttribute.BaseType, baseAttribute.MixinTypes, baseAttribute.CompleteInterfaces,
          baseAttribute.ExplicitDependenciesPerMixin);
    }

    internal static CustomAttributeBuilder BuilderFromClassContext (int mixinIndex, ClassContext context)
    {
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixinTypeAttribute attribute = FromClassContext (mixinIndex, context);
      CustomAttributeBuilder builder = new CustomAttributeBuilder (s_attributeCtor, new object[] { attribute.MixinIndex, attribute.BaseType,
          attribute.MixinTypes, attribute.CompleteInterfaces, attribute.ExplicitDependenciesPerMixin });
      return builder;
    }

    private int _mixinIndex;

    public ConcreteMixinTypeAttribute (int mixinIndex, Type baseType, Type[] mixinTypes, Type[] completeInterfaces, Type[] explicitDependenciesPerMixin)
        : base (baseType, mixinTypes, completeInterfaces, explicitDependenciesPerMixin)
    {
      _mixinIndex = mixinIndex;
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
    }

    public MixinDefinition GetMixinDefinition ()
    {
      return GetBaseClassDefinition ().Mixins[MixinIndex];
    }
  }
}
