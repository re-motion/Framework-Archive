// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Descriptors;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Represents a <see cref="MethodInfo"/> that can be modified.
  /// </summary>
  [DebuggerDisplay ("{ToDebugString(),nq}")]
  public class MutableMethodInfo : MethodInfo, IMutableMethodBase
  {
    private readonly MutableType _declaringType;
    private readonly MethodDescriptor _descriptor;

    private readonly MutableInfoCustomAttributeContainer _customAttributeContainer;
    private readonly ReadOnlyCollection<MutableParameterInfo> _parameters;
    private readonly HashSet<MethodInfo> _addedExplicitBaseDefinitions = new HashSet<MethodInfo>();

    private MethodAttributes _attributes;
    private Expression _body;

    public MutableMethodInfo (MutableType declaringType, MethodDescriptor descriptor)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("descriptor", descriptor);

      _declaringType = declaringType;
      _descriptor = descriptor;

      _customAttributeContainer = new MutableInfoCustomAttributeContainer (descriptor.CustomAttributeDataProvider, () => CanAddCustomAttributes);
      _parameters = _descriptor.ParameterDescriptors.Select (pd => new MutableParameterInfo (this, pd)).ToList().AsReadOnly();

      _attributes = _descriptor.Attributes;
      _body = _descriptor.Body;
    }

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    public MethodInfo UnderlyingSystemMethodInfo
    {
      get { return _descriptor.UnderlyingSystemInfo ?? this; }
    }

    public bool IsNew
    {
      get { return _descriptor.UnderlyingSystemInfo == null; }
    }

    public bool IsModified
    {
      get { return _body != _descriptor.Body || AddedCustomAttributeDeclarations.Count > 0 || _addedExplicitBaseDefinitions.Count > 0; }
    }

    public override string Name
    {
      get { return _descriptor.Name; }
    }

    public override MethodAttributes Attributes
    {
      get { return _attributes; }
    }

    public override CallingConventions CallingConvention
    {
      get { return IsStatic ? CallingConventions.Standard : CallingConventions.HasThis; }
    }

    public override Type ReturnType
    {
      get { return _descriptor.ReturnType; }
    }

    public MethodInfo BaseMethod
    {
      get { return _descriptor.BaseMethod; }
    }

    public override bool IsGenericMethod
    {
      get { return _descriptor.IsGenericMethod; }
    }

    public override bool IsGenericMethodDefinition
    {
      get { return _descriptor.IsGenericMethodDefinition; }
    }

    public override bool ContainsGenericParameters
    {
      get { return _descriptor.ContainsGenericParameters; }
    }

    public ReadOnlyCollection<ParameterExpression> ParameterExpressions
    {
      get { return _descriptor.ParameterDescriptors.Select (pd => pd.Expression).ToList().AsReadOnly(); }
    }

    public bool CanAddCustomAttributes
    {
      // TODO 4695
      get { return CanSetBody; }
    }

    public ReadOnlyCollection<CustomAttributeDeclaration> AddedCustomAttributeDeclarations
    {
      get { return _customAttributeContainer.AddedCustomAttributeDeclarations; }
    }

    public bool CanSetBody
    {
      // TODO 4695
      get { return IsNew || (IsVirtual && !IsFinal); }
    }

    public Expression Body
    {
      get
      {
        if (IsAbstract)
          throw new InvalidOperationException ("An abstract method has no body.");

        return _body;
      }
    }

    public bool CanAddExplicitBaseDefinition
    {
      // TODO 4695; Note that IsVirtual must always be checked here - this is not caused by the Reflection.Emit code generator, but by the CLI rules.
      get { return IsVirtual && (IsNew || !IsFinal); }
    }

    /// <summary>
    /// Returns all root <see cref="MethodInfo"/> instances that were added via <see cref="AddExplicitBaseDefinition"/>.
    /// </summary>
    public ReadOnlyCollectionDecorator<MethodInfo> AddedExplicitBaseDefinitions
    {
      get { return _addedExplicitBaseDefinitions.AsReadOnly (); }
    }

    public override MethodInfo GetBaseDefinition ()
    {
      return BaseMethod != null ? BaseMethod.GetBaseDefinition() : this;
    }

    public override ParameterInfo[] GetParameters ()
    {
      return _parameters.Cast<ParameterInfo>().ToArray();
    }

    /// <summary>
    /// Adds an explicit base definition, i.e., a root <see cref="MethodInfo"/> explicitly overridden by this <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="overriddenMethodBaseDefinition">The overridden method base definition.</param>
    /// <remarks>
    /// This method does not affect <see cref="GetBaseDefinition"/> or <see cref="BaseMethod"/>, both of which only return implicitly overridden 
    /// methods. Methods can override both a single method implicitly and multiple methods explicitly.
    /// </remarks>
    public void AddExplicitBaseDefinition (MethodInfo overriddenMethodBaseDefinition)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethodBaseDefinition", overriddenMethodBaseDefinition);

      if (!CanAddExplicitBaseDefinition)
      {
        // TODO 4695: Adapt message
        var message = string.Format ("Cannot add an explicit base definition to the non-virtual or existing final method '{0}'.", Name);
        throw new NotSupportedException (message);
      }

      if (!overriddenMethodBaseDefinition.IsVirtual || overriddenMethodBaseDefinition.IsFinal)
        throw new ArgumentException ("Method must be virtual and non-final.", "overriddenMethodBaseDefinition");

      if (!MethodSignature.AreEqual (this, overriddenMethodBaseDefinition))
        throw new ArgumentException ("Method signatures must be equal.", "overriddenMethodBaseDefinition");

      if (!_declaringType.IsAssignableTo (overriddenMethodBaseDefinition.DeclaringType))
        throw new ArgumentException ("The overridden method must be from the same type hierarchy.", "overriddenMethodBaseDefinition");

      if (overriddenMethodBaseDefinition.GetBaseDefinition () != overriddenMethodBaseDefinition)
      {
        throw new ArgumentException (
            "The given method must be a root method definition. (Use GetBaseDefinition to get a root method.)",
            "overriddenMethodBaseDefinition");
      }

      // TODO: check all mutable methods not just the current one
      if (_addedExplicitBaseDefinitions.Contains (overriddenMethodBaseDefinition))
        throw new InvalidOperationException ("The given method has already been added to the list of explicit base definitions.");

      _addedExplicitBaseDefinitions.Add (overriddenMethodBaseDefinition);
    }

    public void SetBody (Func<MethodBodyModificationContext, Expression> bodyProvider)
    {
      ArgumentUtility.CheckNotNull ("bodyProvider", bodyProvider);

      if (!CanSetBody)
      {
        // TODO 4695
        var message = string.Format ("The body of the existing non-virtual or final method '{0}' cannot be replaced.", Name);
        throw new NotSupportedException (message);
      }

      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      var context = new MethodBodyModificationContext (_declaringType, ParameterExpressions, _body, IsStatic, BaseMethod, memberSelector);
      var newBody = BodyProviderUtility.GetTypedBody (ReturnType, bodyProvider, context);

      if (_body == null)
      {
        Assertion.IsTrue (IsAbstract);
        _attributes = _attributes.Unset (MethodAttributes.Abstract);
      }

      _body = newBody;
    }

    public void AddCustomAttribute (CustomAttributeDeclaration customAttributeDeclaration)
    {
      ArgumentUtility.CheckNotNull ("customAttributeDeclaration", customAttributeDeclaration);

      _customAttributeContainer.AddCustomAttribute (customAttributeDeclaration);
    }

    public IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return _customAttributeContainer.GetCustomAttributeData();
    }

    public IEnumerable<ICustomAttributeData> GetCustomAttributeData (bool inherit)
    {
      return TypePipeCustomAttributeData.GetCustomAttributes (this, inherit);
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      return CustomAttributeFinder.GetCustomAttributes (this, inherit);
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return CustomAttributeFinder.GetCustomAttributes (this, attributeType, inherit);
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return CustomAttributeFinder.IsDefined (this, attributeType, inherit);
    }

    public override string ToString ()
    {
      return SignatureDebugStringGenerator.GetMethodSignature (this);
    }

    public string ToDebugString ()
    {
      return string.Format ("MutableMethod = \"{0}\", DeclaringType = \"{1}\"", ToString (), DeclaringType);
    }

    #region Not YET Implemented from MethodInfo interface

    public override MethodImplAttributes GetMethodImplementationFlags ()
    {
      throw new NotImplementedException();
    }

    public override ICustomAttributeProvider ReturnTypeCustomAttributes
    {
      get { throw new NotImplementedException(); }
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override RuntimeMethodHandle MethodHandle
    {
      get { throw new NotImplementedException(); }
    }

    public override Type[] GetGenericArguments ()
    {
      throw new NotImplementedException ();
    }

    public override MethodInfo GetGenericMethodDefinition ()
    {
      throw new NotImplementedException ();
    }

    public override MethodInfo MakeGenericMethod (params Type[] typeArguments)
    {
      throw new NotImplementedException ();
    }

    public override ParameterInfo ReturnParameter
    {
      get { throw new NotImplementedException (); }
    }

    public override MethodBody GetMethodBody ()
    {
      throw new NotImplementedException ();
    }

    #endregion

    #region Unsupported Members

    public override int MetadataToken
    {
      get { throw new NotSupportedException ("Property MetadataToken is not supported."); }
    }

    public override Module Module
    {
      get { throw new NotSupportedException ("Property Module is not supported."); }
    }

    public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
      throw new NotSupportedException ("Method Invoke is not supported.");
    }

    #endregion
  }
}