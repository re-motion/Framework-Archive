// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides  information about the attributes of a member and provides access to member metadata.
  /// <seealso cref="IPropertyInformation"/>
  /// <seealso cref="IMethodInformation"/>
  /// </summary>
  public interface IMemberInformation
  {
    /// <summary>
    /// Gets the simple name of the member identifying it within its declaring type.
    /// </summary>
    /// <value>The simple property name.</value>
    string Name { get; }

    /// <summary>
    /// Gets the type declaring the member.
    /// </summary>
    /// <value>The declaring type of the property.</value>
    Type DeclaringType { get; }

    /// <summary>
    /// Gets the type the member was originally declared on.
    /// </summary>
    /// <returns>The type the member was originally declared on.</returns>
    /// <remarks>If the member represented by this instance overrides a member from a base type, this method will return the base type.</remarks>
    Type GetOriginalDeclaringType ();

    /// <summary>
    /// Gets the one custom attribute of type <typeparamref name="T"/> declared on this member, or null if no such attribute exists.
    /// </summary>
    /// <typeparam name="T">The type of attribute to retrieve.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attribute. Otherwise, only the <see cref="DeclaringType"/>
    /// is checked.</param>
    /// <exception cref="AmbiguousMatchException">More than one instance of the given attribute type <typeparamref name="T"/> is declared on this
    /// member.</exception>
    /// <returns>An instance of type <typeparamref name="T"/>, or <see langword="null"/> if no attribute of that type is declared on this member.</returns>
    T GetCustomAttribute<T> (bool inherited) where T : class;

    /// <summary>
    /// Gets the custom attributes of type <typeparamref name="T"/> declared on this member, or null if no such attribute exists.
    /// </summary>
    /// <typeparam name="T">The type of the attributes to retrieve.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attributes. Otherwise, only the <see cref="DeclaringType"/>
    /// is checked.</param>
    /// <returns>An array of the attributes of type <typeparamref name="T"/> declared on this member, or an empty array if no attribute of
    /// that type is declared on this member.</returns>
    T[] GetCustomAttributes<T> (bool inherited) where T : class;

    /// <summary>
    /// Determines whether a custom attribute of the specified type <typeparamref name="T"/> is defined on the member.
    /// </summary>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <param name="inherited">If set to true, the inheritance hierarchy is searched for the attribute. Otherwise, only the type
    /// is checked.</param>
    /// <returns>
    /// True if a custom attribute of the specified type is defined on the member; otherwise, false.
    /// </returns>
    bool IsDefined<T> (bool inherited) where T : class;

    /// <summary>
    /// Finds the implementation <see cref="IMemberInformation"/> corresponding to this <see cref="IMemberInformation"/> on the given 
    /// <see cref="Type"/>. This <see cref="IMemberInformation"/> object must denote an interface property.
    /// </summary>
    /// <param name="implementationType">The type to search for an implementation of this <see cref="IMemberInformation"/> on.</param>
    /// <returns>An instance of <see cref="IMemberInformation"/> describing the member implementing this interface 
    /// <see cref="IMemberInformation"/> on <paramref name="implementationType"/>, or <see langword="null" /> if the 
    /// <paramref name="implementationType"/> does not implement the interface.</returns>
    IMemberInformation FindInterfaceImplementation (Type implementationType);

    /// <summary>
    /// Finds the member declaration for the <see cref="IMemberInformation"/>. This <see cref="IMemberInformation"/> object must denote an 
    /// implementation member. 
    /// </summary>
    /// <returns>Returns the <see cref="IMemberInformation"/> of the declared member or null if no corresponding member declaration was found.</returns>
    IMemberInformation FindInterfaceDeclaration ();
  }
}