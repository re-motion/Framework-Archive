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
      else
        ; // TODO
    }

    private void ApplyForImplementedInterface ()
    {
      InterfaceMapping interfaceMapping = _declaringRequirement.BaseClass.GetAdjustedInterfaceMap (_declaringRequirement.Type);
      for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = interfaceMapping.InterfaceMethods[i];
        MethodDefinition implementingMethod;
        if (_propertyGetters.ContainsKey (interfaceMethod))
          implementingMethod = FindProperty (_propertyGetters[interfaceMethod], _declaringRequirement.BaseClass.Properties).GetMethod;
        else if (_propertySetters.ContainsKey (interfaceMethod))
          implementingMethod = FindProperty (_propertySetters[interfaceMethod], _declaringRequirement.BaseClass.Properties).SetMethod;
        else if (_eventAdders.ContainsKey (interfaceMethod))
          implementingMethod = FindEvent (_eventAdders[interfaceMethod], _declaringRequirement.BaseClass.Events).AddMethod;
        else if (_eventRemovers.ContainsKey (interfaceMethod))
          implementingMethod = FindEvent (_eventRemovers[interfaceMethod], _declaringRequirement.BaseClass.Events).RemoveMethod;
        else
          implementingMethod = _declaringRequirement.BaseClass.Methods[interfaceMapping.TargetMethods[i]];

        Assertion.Assert (implementingMethod != null);

        _declaringRequirement.Methods.Add (new RequiredMethodDefinition (_declaringRequirement, interfaceMethod, implementingMethod));
      }
    }

    private PropertyDefinition FindProperty (PropertyInfo interfaceProperty, UniqueDefinitionCollection<PropertyInfo, PropertyDefinition> properties)
    {
      foreach (PropertyDefinition property in properties)
      {
        if (s_propertyComparer.Equals (property.PropertyInfo, interfaceProperty))
        {
          return property;
        }
      }
      return null;
    }

    private EventDefinition FindEvent (EventInfo interfaceEvent, UniqueDefinitionCollection<EventInfo, EventDefinition> events)
    {
      foreach (EventDefinition eventInfo in events)
      {
        if (s_eventComparer.Equals (eventInfo.EventInfo, interfaceEvent))
        {
          return eventInfo;
        }
      }
      return null;
    }
  }
}
