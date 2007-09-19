using System;
using System.Reflection;
using System.Reflection.Emit;
using Rubicon.CodeGeneration;
using Rubicon.Utilities;

namespace Rubicon.CodeGeneration.DPExtensions
{
  public static class AttributeReplicator
  {
    public static void ReplicateAttribute (IAttributableEmitter target, CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilder builder = ReflectionEmitUtility.CreateAttributeBuilderFromData (attributeData);
      target.AddCustomAttribute (builder);
    }
  }
}
