using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.Utilities;
using Rubicon.Collections;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.Definitions.Building
{
  public class MemberDefinitionBuilder
  {
    private readonly ClassDefinitionBase _classDefinition;
    private readonly Predicate<MethodInfo> _methodFilter;
    private readonly Set<MethodInfo> _specialMethods = new Set<MethodInfo> ();
    private readonly BindingFlags _bindingFlags;

    public MemberDefinitionBuilder (ClassDefinitionBase classDefinition, Predicate<MethodInfo> methodFilter, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("methodFilter", methodFilter);

      _classDefinition = classDefinition;
      _methodFilter = methodFilter;
      _bindingFlags = bindingFlags;
    }

    public void Apply (Type type)
    {
      IEnumerable<MethodInfo> methods = ReflectionUtility.RecursiveGetAllMethods (type, _bindingFlags);
      IEnumerable<PropertyInfo> properties = ReflectionUtility.RecursiveGetAllProperties (type, _bindingFlags);
      IEnumerable<EventInfo> events = ReflectionUtility.RecursiveGetAllEvents (type, _bindingFlags);

      AnalyzeProperties (CleanupMembers(properties, new PropertyNameAndSignatureEqualityComparer()));
      AnalyzeEvents (CleanupMembers(events, new EventNameAndSignatureEqualityComparer()));
      AnalyzeMethods (CleanupMembers(methods, new MethodNameAndSignatureEqualityComparer()));
    }

    private IEnumerable<T> CleanupMembers<T> (IEnumerable<T> members, IEqualityComparer<T> comparer) where T : MemberInfo
    {
      MultiSet<T> memberSet = new MultiSet<T>(comparer);
      memberSet.AddRange (members); // collect duplicates
      Set<T> result = new Set<T>();
      
      foreach (T member in members)
      {
        MemberChain<T> chain = new MemberChain<T> (memberSet[member]);
        result.AddRange (chain.GetNonOverriddenMembers());
      }
      return result;
    }

    private void AnalyzeProperties (IEnumerable<PropertyInfo> properties)
    {
      foreach (PropertyInfo property in properties)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);

        MethodDefinition getMethodDefinition = CreateSpecialMethodDefinition (getMethod);
        MethodDefinition setMethodDefinition = CreateSpecialMethodDefinition (setMethod);

        if (getMethodDefinition != null || setMethodDefinition != null)
        {
          PropertyDefinition definition = new PropertyDefinition (property, _classDefinition, getMethodDefinition, setMethodDefinition);
          AttributeDefinitionBuilder attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (CustomAttributeData.GetCustomAttributes (property));
          _classDefinition.Properties.Add (definition);
        }
      }
    }

    private void AnalyzeEvents (IEnumerable<EventInfo> events)
    {
      foreach (EventInfo eventInfo in events)
      {
        MethodInfo addMethod = eventInfo.GetAddMethod (true);
        MethodInfo removeMethod = eventInfo.GetRemoveMethod (true);

        MethodDefinition addMethodDefinition = CreateSpecialMethodDefinition (addMethod);
        MethodDefinition removeMethodDefinition = CreateSpecialMethodDefinition (removeMethod);

        if (addMethodDefinition != null || removeMethodDefinition != null)
        {
          EventDefinition definition = new EventDefinition (eventInfo, _classDefinition, addMethodDefinition, removeMethodDefinition);
          AttributeDefinitionBuilder attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (CustomAttributeData.GetCustomAttributes (eventInfo));
          _classDefinition.Events.Add (definition);
        }
      }
    }

    private void AnalyzeMethods (IEnumerable<MethodInfo> methods)
    {
      foreach (MethodInfo method in methods)
      {
        if (!_specialMethods.Contains (method) && _methodFilter (method))
        {
          MethodDefinition definition = new MethodDefinition (method, _classDefinition);
          AttributeDefinitionBuilder attributeBuilder = new AttributeDefinitionBuilder (definition);
          attributeBuilder.Apply (CustomAttributeData.GetCustomAttributes (method));
          _classDefinition.Methods.Add (definition);
        }
      }
    }

    private MethodDefinition CreateSpecialMethodDefinition (MethodInfo methodInfo)
    {
      if (methodInfo != null && _methodFilter (methodInfo))
      {
        MethodDefinition methodDefinition = new MethodDefinition (methodInfo, _classDefinition);
        AttributeDefinitionBuilder attributeBuilder = new AttributeDefinitionBuilder (methodDefinition);
        attributeBuilder.Apply (CustomAttributeData.GetCustomAttributes (methodInfo));
        _specialMethods.Add (methodInfo);
        return methodDefinition;
      }
      else
        return null;
    }

    public static void InitializeAttributes (ClassDefinitionBase classDefinition)
    {
      foreach (CustomAttributeData attributeData in CustomAttributeData.GetCustomAttributes(classDefinition.Type))
      {
        classDefinition.CustomAttributes.Add (new AttributeDefinition (classDefinition, attributeData));
      }
    }
  }
}
