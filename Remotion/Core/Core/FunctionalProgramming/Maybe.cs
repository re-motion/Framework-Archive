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
using Remotion.Utilities;

namespace Remotion.FunctionalProgramming
{
  /// <summary>
  /// Provides non-generic helper methods for the <see cref="Maybe{T}"/> type.
  /// </summary>
  public static class Maybe
  {
    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given value, which can be <see langword="null" />.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="valueOrNull"/>.</typeparam>
    /// <param name="valueOrNull">The value. Can be <see langword="null" />, in which case <see cref="Maybe{T}.Nothing"/> is returned.</param>
    /// <returns><see cref="Maybe{T}.Nothing"/> if <paramref name="valueOrNull"/> is <see langword="null" />; otherwise an instance of 
    /// <see cref="Maybe{T}"/> that encapsulates the given <paramref name="valueOrNull"/>.</returns>
    public static Maybe<T> FromValue<T> (T valueOrNull)
    {
      return new Maybe<T> (valueOrNull);
    }

    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> instance for the given <paramref name="nullableValue"/>, unwrapping a nullable value type.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <paramref name="nullableValue"/>.</typeparam>
    /// <param name="nullableValue">The nullable value.</param>
    /// <returns><see cref="Maybe{T}.Nothing"/> if <paramref name="nullableValue"/> is <see langword="null" />; otherwise an instance of 
    /// <see cref="Maybe{T}"/> that encapsulates the underlying value of <paramref name="nullableValue"/>.</returns>
    public static Maybe<T> FromNullableValueType<T> (T? nullableValue) where T : struct
    {
      return nullableValue.HasValue ? new Maybe<T> (nullableValue.Value) : Maybe<T>.Nothing;
    }
  }

  /// <summary>
  /// Encapsulates a value that may be <see langword="null" />, providing helpful methods to avoid <see langword="null" /> checks.
  /// </summary>
  public struct Maybe<T>
  {
    public static readonly Maybe<T> Nothing = new Maybe<T> ();

    private readonly T _value;
    private readonly bool _hasValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Maybe{T}"/> struct.
    /// </summary>
    /// <param name="value">
    /// The value. If the value is <see langword="null" />, the created instance will compare equal to <see cref="Nothing"/>.
    /// </param>
    public Maybe (T value)
    {
      _value = value;
      // ReSharper disable CompareNonConstrainedGenericWithNull
      _hasValue = value != null;
      // ReSharper restore CompareNonConstrainedGenericWithNull
    }

    /// <summary>
    /// Gets a value indicating whether this instance has a value.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has a value; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasValue
    {
      get { return _hasValue; }
    }

    /// <summary>
    /// Provides a human-readable representation of this instance.
    /// </summary>
    /// <returns>
    /// A human-readable representation of this instance.
    /// </returns>
    public override string ToString ()
    {
      return string.Format ("{0} ({1})", (HasValue ? "Value: " + _value : "Nothing"), typeof (T).Name);
    }

    /// <summary>
    /// Selects another value from this instance. If this instance does not have a value, the selected value is <see cref="Nothing"/>.
    /// Otherwise, the selected value is retrieved via a selector function.
    /// </summary>
    /// <typeparam name="TR">The type of the value to be selected.</typeparam>
    /// <param name="selector">The selector function. This function is only executed if this instance has a value.</param>
    /// <returns><see cref="Nothing"/> if this instance has no value or <paramref name="selector"/> returns <see langword="null" />; 
    /// otherwise, a new <see cref="Maybe{T}"/> instance holding the value returned by <paramref name="selector"/>.
    /// </returns>
    public Maybe<TR> Select<TR> (Func<T, TR> selector)
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      if (_hasValue)
        return Maybe.FromValue (selector (_value));
      else
        return Maybe<TR>.Nothing;
    }

    /// <summary>
    /// Selects a nullable value from this instance. If this instance does not have a value, the selected value is <see cref="Nothing"/>.
    /// Otherwise, the selected value is retrieved via a selector function.
    /// </summary>
    /// <typeparam name="TR">The type of the value to be selected.</typeparam>
    /// <param name="selector">The selector function. This function is only executed if this instance has a value. Its return value is unwrapped
    /// into the underlying type.</param>
    /// <returns><see cref="Nothing"/> if this instance has no value or <paramref name="selector"/> returns <see langword="null" />; 
    /// otherwise, a new <see cref="Maybe{T}"/> instance holding the non-nullable value returned by <paramref name="selector"/>.
    /// </returns>
    public Maybe<TR> Select<TR> (Func<T, TR?> selector) where TR : struct
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      if (_hasValue)
        return Maybe.FromNullableValueType (selector (_value));
      else
        return Maybe<TR>.Nothing;
    }

    /// <summary>
    /// Gets the value held by this instance, or <see langword="null" /> if this instance is <see cref="Nothing"/>.
    /// </summary>
    /// <returns>The value held by this instance, or <see langword="null" /> if this instance is <see cref="Nothing"/></returns>
    public T GetValueOrNull ()
    {
      return _value;
    }
  }
}