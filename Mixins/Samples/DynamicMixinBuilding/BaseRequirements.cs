using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Rubicon.CodeGeneration;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Samples.DynamicMixinBuilding
{
  internal class BaseRequirements
  {
    public readonly Type RequirementsType;

    private readonly IDictionary<MethodInfo, MethodInfo> _methodToInterfaceMap;

    private BaseRequirements (Type requirementsType, IDictionary<MethodInfo, MethodInfo> methodToInterfaceMap)
    {
      ArgumentUtility.CheckNotNull ("requirementsType", requirementsType);
      ArgumentUtility.CheckNotNull ("methodToInterfaceMap", methodToInterfaceMap);

      RequirementsType = requirementsType;
      _methodToInterfaceMap = methodToInterfaceMap;
    }

    public MethodInfo GetBaseCallMethod (MethodInfo targetMethod)
    {
      return _methodToInterfaceMap[targetMethod];
    }

    public static BaseRequirements BuildBaseRequirements (IEnumerable<MethodInfo> methodsToOverride, string typeName, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("methodsToOverride", methodsToOverride);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("scope", scope);

      CustomClassEmitter requirementsInterface = new CustomClassEmitter (new InterfaceEmitter (scope, typeName));
      
      Dictionary<MethodInfo, MethodInfo> methodToInterfaceMap = new Dictionary<MethodInfo, MethodInfo> ();
      foreach (MethodInfo method in methodsToOverride)
      {
        MethodInfo interfaceMethod = DefineEquivalentInterfaceMethod (requirementsInterface, method);
        methodToInterfaceMap.Add (method, interfaceMethod);
      }

      BaseRequirements result = new BaseRequirements (requirementsInterface.BuildType (), methodToInterfaceMap);
      return result;
    }

    private static MethodInfo DefineEquivalentInterfaceMethod (CustomClassEmitter emitter, MethodInfo method)
    {
      CustomMethodEmitter interfaceMethod = emitter.CreateMethod (method.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);
      interfaceMethod.CopyParametersAndReturnType (method);
      return interfaceMethod.MethodBuilder;
    }
  }
}