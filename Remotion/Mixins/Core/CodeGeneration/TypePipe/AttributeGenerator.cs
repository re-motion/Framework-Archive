// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class AttributeGenerator : IAttributeGenerator
  {
    private static readonly ConstructorInfo s_debuggerBrowsableAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerBrowsableAttribute (DebuggerBrowsableState.Never));

    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerDisplayAttribute ("display message"));
    private static readonly PropertyInfo s_debuggerDisplayAttributeNameProperty =
        MemberInfoFromExpressionUtility.GetProperty ((DebuggerDisplayAttribute o) => o.Name);

    private static readonly ConstructorInfo s_introducedMemberAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new IntroducedMemberAttribute (null, "mixinMemberName", null, "interfaceMemberName"));

    public void AddDebuggerBrowsableAttribute (IMutableMember member, DebuggerBrowsableState debuggerBrowsableState)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var attribute = new CustomAttributeDeclaration (s_debuggerBrowsableAttributeConstructor, new object[] { debuggerBrowsableState });
      member.AddCustomAttribute (attribute);
    }

    public void AddDebuggerDisplayAttribute (IMutableMember member, string debuggerDisplayString, string debuggerDisplayNameStringOrNull)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNullOrEmpty ("debuggerDisplayString", debuggerDisplayString);
      // Debugger display name may be null.

      var attribute = new CustomAttributeDeclaration (
          s_debuggerDisplayAttributeConstructor,
          new object[] { debuggerDisplayString },
          new NamedArgumentDeclaration (s_debuggerDisplayAttributeNameProperty, debuggerDisplayNameStringOrNull));
      member.AddCustomAttribute (attribute);
    }

    public void AddIntroducedMemberAttribute (IMutableMember member, MemberInfo interfaceMember, MemberDefinitionBase implementingMember)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      var attribute = new CustomAttributeDeclaration (
          s_introducedMemberAttributeConstructor,
          new object[] { implementingMember.DeclaringClass.Type, implementingMember.Name, interfaceMember.DeclaringType, interfaceMember.Name });
      member.AddCustomAttribute (attribute);
    }

    public void AddMixedTypeAttribute (IMutableMember member, ClassContext classContext, IEnumerable<Type> orderedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("orderedMixinTypes", orderedMixinTypes);
      
      // TODO
    }

    public void AddAttribute (IMutableMember member, ICustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      var attribute = new CustomAttributeDeclaration (
          attributeData.Constructor,
          attributeData.ConstructorArguments.ToArray(),
          attributeData.NamedArguments.Select (CreateNamedArgumentDeclaration).ToArray());
      member.AddCustomAttribute (attribute);
    }

    public void ReplicateAttributes (IAttributableDefinition source, IMutableMember destination)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("destination", destination);

      // TODO
    }

    public bool ShouldBeReplicated (AttributeDefinition attribute, IAttributeIntroductionTarget targetConfiguration, TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("targetConfiguration", targetConfiguration);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      return !attribute.IsCopyTemplate
             && (!CanInheritAttributesFromBase (targetConfiguration)
                 || (!AttributeUtility.IsAttributeInherited (attribute.AttributeType) && !IsSuppressedByMixin (attribute, targetClassDefinition)));
    }

    public virtual bool IsSuppressedByMixin (AttributeDefinition attribute, TargetClassDefinition targetClassDefinition)
    {
      var declaringEntity = attribute.DeclaringDefinition.CustomAttributeProvider;
      var suppressAttributesAttributes =
          from suppressAttribute in targetClassDefinition.ReceivedAttributes[typeof (SuppressAttributesAttribute)]
          let suppressAttributeInstance = (SuppressAttributesAttribute) suppressAttribute.Attribute.Instance
          let suppressingEntity = suppressAttribute.Attribute.DeclaringDefinition.CustomAttributeProvider
          where suppressAttributeInstance.IsSuppressed (attribute.AttributeType, declaringEntity, suppressingEntity)
          select suppressAttributeInstance;
      return suppressAttributesAttributes.Any();
    }

    private bool CanInheritAttributesFromBase (IAttributeIntroductionTarget configuration)
    {
      // Only methods and base classes can supply attributes for inheritance.
      return configuration is TargetClassDefinition || configuration is MethodDefinition;
    }

    private NamedArgumentDeclaration CreateNamedArgumentDeclaration (ICustomAttributeNamedArgument namedArgument)
    {
      if (namedArgument.MemberInfo is FieldInfo)
        return new NamedArgumentDeclaration ((FieldInfo) namedArgument.MemberInfo, namedArgument.Value);
      else
        return new NamedArgumentDeclaration ((PropertyInfo) namedArgument.MemberInfo, namedArgument.Value);
    }
  }
}