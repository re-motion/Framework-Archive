using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Collections;
using Rubicon.Mixins.Utilities;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class RequiredMethodDefinitionBuilder
  {
    private readonly RequirementDefinitionBase _declaringRequirement;
    private readonly static MethodNameAndSignatureEqualityComparer s_methodComparer = new MethodNameAndSignatureEqualityComparer ();
    private readonly static PropertyNameAndSignatureEqualityComparer s_propertyComparer = new PropertyNameAndSignatureEqualityComparer ();
    private readonly static EventNameAndSignatureEqualityComparer s_eventComparer = new EventNameAndSignatureEqualityComparer ();

    private Dictionary<MethodInfo, PropertyInfo> _propertyGetters;
    private Dictionary<MethodInfo, PropertyInfo> _propertySetters;
    private Dictionary<MethodInfo, EventInfo> _eventAdders;
    private Dictionary<MethodInfo, EventInfo> _eventRemovers;

    public RequiredMethodDefinitionBuilder (RequirementDefinitionBase declaringRequirement)
    {
      _declaringRequirement = declaringRequirement;

      // since clients can only use public members of required types (via This and Base calls), we only need to consider those public members
      _propertyGetters = new Dictionary<MethodInfo, PropertyInfo> ();
      _propertySetters = new Dictionary<MethodInfo, PropertyInfo> ();
      foreach (PropertyInfo property in declaringRequirement.Type.GetProperties ())
      {
        MethodInfo getMethod = property.GetGetMethod ();
        if (getMethod != null)
          _propertyGetters.Add (getMethod, property);
        MethodInfo setMethod = property.GetSetMethod ();
        if (setMethod != null)
          _propertySetters.Add (setMethod, property);
      }

      _eventAdders = new Dictionary<MethodInfo, EventInfo> ();
      _eventRemovers = new Dictionary<MethodInfo, EventInfo> ();
      foreach (EventInfo eventInfo in declaringRequirement.Type.GetEvents ())
      {
        _eventAdders.Add (eventInfo.GetAddMethod (), eventInfo);
        _eventRemovers.Add (eventInfo.GetRemoveMethod (), eventInfo);
      }
    }

    public void Apply ()
    {
      if (_declaringRequirement.IsEmptyInterface)
        return;

      if (_declaringRequirement.BaseClass.ImplementedInterfaces.Contains (_declaringRequirement.Type))
        ApplyForImplementedInterface ();
      else if (_declaringRequirement.BaseClass.IntroducedInterfaces.ContainsKey (_declaringRequirement.Type))
        ApplyForIntroducedInterface ();
      else
        ApplyWithDuckTyping ();
    }

    private void ApplyForImplementedInterface ()
    {
      Dictionary<MethodInfo, MethodDefinition> allMethods = new Dictionary<MethodInfo, MethodDefinition> ();
      foreach (MethodDefinition methodDefinition in _declaringRequirement.BaseClass.GetAllMethods())
        allMethods.Add (methodDefinition.MethodInfo, methodDefinition);

      InterfaceMapping interfaceMapping = _declaringRequirement.BaseClass.GetAdjustedInterfaceMap (_declaringRequirement.Type);
      for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = interfaceMapping.InterfaceMethods[i];
        MethodDefinition implementingMethod = allMethods[interfaceMapping.TargetMethods[i]];

        AddRequiredMethod (interfaceMethod, implementingMethod);
      }
    }

    private void ApplyForIntroducedInterface ()
    {
      InterfaceIntroductionDefinition introduction = _declaringRequirement.BaseClass.IntroducedInterfaces[_declaringRequirement.Type];
      foreach (EventIntroductionDefinition eventIntroduction in introduction.IntroducedEvents)
      {
        AddRequiredMethod (eventIntroduction.InterfaceMember.GetAddMethod(), eventIntroduction.ImplementingMember.AddMethod);
        AddRequiredMethod (eventIntroduction.InterfaceMember.GetRemoveMethod(), eventIntroduction.ImplementingMember.RemoveMethod);
      }
      foreach (PropertyIntroductionDefinition propertyIntroduction in introduction.IntroducedProperties)
      {
        AddRequiredMethod (propertyIntroduction.InterfaceMember.GetGetMethod(), propertyIntroduction.ImplementingMember.GetMethod);
        AddRequiredMethod (propertyIntroduction.InterfaceMember.GetSetMethod(), propertyIntroduction.ImplementingMember.SetMethod);
      }
      foreach (MethodIntroductionDefinition methodIntroduction in introduction.IntroducedMethods)
        AddRequiredMethod (methodIntroduction.InterfaceMember, methodIntroduction.ImplementingMember);
    }

    private void ApplyWithDuckTyping ()
    {
      foreach (MethodInfo interfaceMethod in _declaringRequirement.Type.GetMethods())
      {
        MethodDefinition implementingMethod = FindMethodOnBaseIncludingSpecials (interfaceMethod);
        AddRequiredMethod (interfaceMethod, implementingMethod);
      }
    }

    private void AddRequiredMethod (MethodInfo interfaceMethod, MethodDefinition implementingMethod)
    {
      if (interfaceMethod != null)
      {
        Assertion.Assert (implementingMethod != null);
        _declaringRequirement.Methods.Add (new RequiredMethodDefinition (_declaringRequirement, interfaceMethod, implementingMethod));
      }
    }

    private MethodDefinition FindMethodOnBaseIncludingSpecials (MethodInfo interfaceMethod)
    {
      if (_propertyGetters.ContainsKey (interfaceMethod))
        return FindProperty (_propertyGetters[interfaceMethod], _declaringRequirement.BaseClass.Properties).GetMethod;
      else if (_propertySetters.ContainsKey (interfaceMethod))
        return FindProperty (_propertySetters[interfaceMethod], _declaringRequirement.BaseClass.Properties).SetMethod;
      else if (_eventAdders.ContainsKey (interfaceMethod))
        return FindEvent (_eventAdders[interfaceMethod], _declaringRequirement.BaseClass.Events).AddMethod;
      else if (_eventRemovers.ContainsKey (interfaceMethod))
        return FindEvent (_eventRemovers[interfaceMethod], _declaringRequirement.BaseClass.Events).RemoveMethod;
      else
        return FindMethod (interfaceMethod, _declaringRequirement.BaseClass.Methods);
    }

    private MethodDefinition FindMethod (MethodInfo interfaceMethod, UniqueDefinitionCollection<MethodInfo, MethodDefinition> methods)
    {
      foreach (MethodDefinition method in methods)
      {
        if (s_methodComparer.Equals (method.MethodInfo, interfaceMethod))
          return method;
      }

      throw ConstructExceptionOnMemberNotFound ("method " + interfaceMethod.Name);
    }

    private PropertyDefinition FindProperty (PropertyInfo interfaceProperty, UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> properties)
    {
      foreach (PropertyDefinition property in properties)
      {
        if (s_propertyComparer.Equals (property.PropertyInfo, interfaceProperty))
          return property;
      }

      throw ConstructExceptionOnMemberNotFound ("property " + interfaceProperty.Name);
    }

    private EventDefinition FindEvent (EventInfo interfaceEvent, UniqueDefinitionCollection<EventInfo, EventDefinition> events)
    {
      foreach (EventDefinition eventInfo in events)
      {
        if (s_eventComparer.Equals (eventInfo.EventInfo, interfaceEvent))
          return eventInfo;
      }

      throw ConstructExceptionOnMemberNotFound ("event " + interfaceEvent.Name);
    }

    private Exception ConstructExceptionOnMemberNotFound (string memberString)
    {
      string dependenciesString = CollectionStringBuilder.BuildCollectionString (
          _declaringRequirement.FindRequiringMixins(),
          ", ",
          delegate (MixinDefinition m) { return m.FullName; });
      string message = string.Format (
          "The dependency {0} (mixins {1} applied to class {2}) is not fulfilled - public or protected {3} could not be "
          + "found on the base class.",
          _declaringRequirement.Type.Name,
          dependenciesString,
          _declaringRequirement.BaseClass.FullName,
          memberString);
      throw new ConfigurationException (message);
    }
  }
}
