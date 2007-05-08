using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public static class AttributeReplicator
  {
    public static void ReplicateAttribute (IAttributableEmitter target, CustomAttributeData attributeData)
    {
      CustomAttributeBuilder builder = ReflectionEmitUtility.CreateAttributeBuilderFromData (attributeData);
      target.AddCustomAttribute (builder);
    }
  }
}
