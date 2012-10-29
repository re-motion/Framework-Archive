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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines that a mixin is not applied to a specific class, even when it is explicitly or implicitly configured for that class via
  /// the declarative configuration attributes <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and <see cref="MixAttribute"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Use this attribute to exclude a target class inherited from a mixin's base class. This attribute is not inherited, so the target class
  /// exclusion will only work for the exact mixin to which the attribute is applied.
  /// </para>
  /// <para>
  /// Note that when a mixin excludes a generic type definition (e.g. <c>C&lt;&gt;</c>), a corresponding closed generic type (<c>C&lt;int&gt;</c>) can
  /// still inherit the mixin from its base class. This is by design due to the rule that a closed generic type inherits mixins from both 
  /// its base class and its generic type definition.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class IgnoresClassAttribute : Attribute, IMixinConfigurationAttribute<Type>
  {
    private readonly Type _classToIgnore;

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoresClassAttribute"/> class, specifying the class to be ignored by this mixin.
    /// </summary>
    /// <param name="classToIgnore">The class to be ignored in declarative configuration. Subclasses of this class
    /// will not inherit the mixin either.</param>
    public IgnoresClassAttribute (Type classToIgnore)
    {
      _classToIgnore = classToIgnore;
    }

    /// <summary>
    /// Gets the class to be ignored by this mixin.
    /// </summary>
    /// <value>The class to be ignored.</value>
    public Type ClassToIgnore
    {
      get { return _classToIgnore; }
    }

    public void Apply (MixinConfigurationBuilder mixinConfigurationBuilder, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinConfigurationBuilder", mixinConfigurationBuilder);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      mixinConfigurationBuilder.ForClass (ClassToIgnore).SuppressMixin (mixinType);
    }
  }
}
