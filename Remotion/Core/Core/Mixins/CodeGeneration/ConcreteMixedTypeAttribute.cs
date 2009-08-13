// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Applied to concrete mixed types generated by the mixin engine.
  /// </summary>
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ConcreteMixedTypeAttribute : Attribute
  {
    /// <summary>
    /// Creates a <see cref="ConcreteMixedTypeAttribute"/> from a given <see cref="ClassContext"/>.
    /// </summary>
    /// <param name="context">The class context describing the concrete mixed type.</param>
    /// <param name="orderedMixinTypes">The types of the mixins applied to the target class of this attribute in the same order that was used for the 
    /// code generation. The mixin types directly match the mixin types defined by <see cref="TargetClassDefinition.Mixins"/>.</param>
    /// <returns>A new <see cref="ConcreteMixedTypeAttribute"/> with the <see cref="ClassContextData"/> property holding a serialized version of
    /// <paramre name="context"/>.</returns>
    public static ConcreteMixedTypeAttribute FromClassContext (ClassContext context, Type[] orderedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var serializer = new AttributeClassContextSerializer ();
      context.Serialize (serializer);

      return new ConcreteMixedTypeAttribute (serializer.Values, orderedMixinTypes);
    }

    private readonly object[] _classContextData;
    private readonly Type[] _orderedMixinTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcreteMixedTypeAttribute"/> class.
    /// </summary>
    /// <param name="classContextData">The serialized class context data, produced by <see cref="AttributeClassContextSerializer"/>.</param>
    /// <param name="orderedMixinTypes">The types of the mixins applied to the target class of this attribute in the same order that was used for the 
    /// code generation. The mixin types directly match the mixin types defined by <see cref="TargetClassDefinition.Mixins"/>.</param>
    public ConcreteMixedTypeAttribute (object[] classContextData, Type[] orderedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("classContextData", classContextData);
      ArgumentUtility.CheckNotNull ("orderedMixinTypes", orderedMixinTypes);

      _classContextData = classContextData;
      _orderedMixinTypes = orderedMixinTypes;
    }

    /// <summary>
    /// Gets the serialized class context data, readable by <see cref="AttributeClassContextDeserializer"/>. Use <see cref="GetClassContext"/> to
    /// deserialize this.
    /// </summary>
    /// <value>The serialized class context data.</value>
    public object[] ClassContextData
    {
      get { return _classContextData; }
    }

    /// <summary>
    /// Gets the types of the mixins applied to the target class of this attribute in the same order that was used for the code generation. The mixin
    /// types reflected by this property directly match the mixin types defined by <see cref="TargetClassDefinition.Mixins"/>.
    /// </summary>
    /// <value>The ordered mixin types.</value>
    public Type[] OrderedMixinTypes
    {
      get { return _orderedMixinTypes; }
    }

    /// <summary>
    /// Gets the class context describing the target type of this attribute.
    /// </summary>
    /// <returns>A deserialized form of <see cref="ClassContextData"/>.</returns>
    public ClassContext GetClassContext ()
    {
      var deserializer = new AttributeClassContextDeserializer (ClassContextData);
      return ClassContext.Deserialize (deserializer);
    }
  }
}
