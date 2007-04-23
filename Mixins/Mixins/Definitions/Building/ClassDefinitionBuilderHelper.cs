using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  static class ClassDefinitionBuilderHelper
  {
    public static void InitializeMembers (ClassDefinition classDefinition, Predicate<MethodInfo> methodFilter)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("methodFilter", methodFilter);

      SpecialMethodSet specialMethods = new SpecialMethodSet();

      foreach (PropertyInfo property in classDefinition.Type.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);

        MethodDefinition getMethodDefinition = GetDefinition (getMethod, classDefinition, methodFilter, specialMethods);
        MethodDefinition setMethodDefinition = GetDefinition (setMethod, classDefinition, methodFilter, specialMethods);

        if (getMethodDefinition != null || setMethodDefinition != null)
          classDefinition.Properties.Add (new PropertyDefinition (property, classDefinition, getMethodDefinition, setMethodDefinition));
      }

      foreach (EventInfo eventInfo in classDefinition.Type.GetEvents (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        MethodInfo addMethod = eventInfo.GetAddMethod (true);
        MethodInfo removeMethod = eventInfo.GetRemoveMethod (true);

        MethodDefinition addMethodDefinition = GetDefinition (addMethod, classDefinition, methodFilter, specialMethods);
        MethodDefinition removeMethodDefinition = GetDefinition (removeMethod, classDefinition, methodFilter, specialMethods);

        if (addMethodDefinition != null || removeMethodDefinition != null)
          classDefinition.Events.Add (new EventDefinition (eventInfo, classDefinition, addMethodDefinition, removeMethodDefinition));
      }

      foreach (MethodInfo method in classDefinition.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (!specialMethods.Contains (method) && methodFilter (method))
          classDefinition.Methods.Add (new MethodDefinition (method, classDefinition));
      }
    }

    private static MethodDefinition GetDefinition (
        MethodInfo methodInfo, ClassDefinition classDefinition, Predicate<MethodInfo> methodFilter, SpecialMethodSet specialMethods)
    {
      if (methodInfo != null && methodFilter (methodInfo))
      {
        MethodDefinition methodDefinition = new MethodDefinition (methodInfo, classDefinition);
        specialMethods.Add (methodInfo);
        return methodDefinition;
      }
      else
        return null;
    }
  }
}
