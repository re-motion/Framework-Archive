using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Utilities
{
  /// <summary>
  /// Provides methods for determining equality and hash codes.
  /// </summary>
  public static class EqualityUtility
  {
    /// <summary>
    /// Gets an object's hash code or null, if the object is <see langword="null"/>.
    /// </summary>
    public static int SafeGetHashCode<T> (T obj)
    {
      return (obj == null) ? 0 : obj.GetHashCode ();
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1> (A0 a0, A1 a1)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1, A2> (A0 a0, A1 a1, A2 a2)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a2);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1, A2, A3> (A0 a0, A1 a1, A2 a2, A3 a3)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a2);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a3);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1, A2, A3, A4> (A0 a0, A1 a1, A2 a2, A3 a3, A4 a4)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a2);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a3);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a4);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1, A2, A3, A4, A5> (A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a2);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a3);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a4);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a5);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode<A0, A1, A2, A3, A4, A5, A6> (A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      int hc = SafeGetHashCode (a0);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a1);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a2);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a3);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a4);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a5);
      Rotate (ref hc);
      hc ^= SafeGetHashCode (a6);
      return hc;
    }

    /// <include file='doc\include\include.xml' path='Comments/EqualityUtility/GetRotatedHashCode/*' />
    public static int GetRotatedHashCode (params object[] fields)
    {
      int hc = 0;
      for (int i = 0; i < fields.Length; ++i)
      {
        object value = fields[i];
        if (value != null)
        {
          hc ^= value.GetHashCode ();
          Rotate (ref hc);
        }
      }
      return hc;
    }

    /// <summary>
    /// Gets the rotated hash code for an enumeration of objects.
    /// </summary>
    /// <param name="objects">The objects whose combined hash code should be calculated.</param>
    /// <returns>The rotate-combined hash codes of the <paramref name="objects"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="objects"/> parameter was <see langword="null"/>.</exception>
    public static int GetRotatedHashCode (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);
      int hc = 0;
      foreach (object value in objects)
      {
        hc ^= SafeGetHashCode (value);
        Rotate (ref hc);
      }
      return hc;
    }

    private static void Rotate (ref int value)
    {
      const int rotateBy = 11;
      value = (value << rotateBy) ^ (value >> (32 - rotateBy));
    }

    /// <summary>
    /// Returns whether two equatable objects are equal.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="Equals{T}"/>, but without any boxing (better performance). 
    /// Equatable objects implement the <see cref="IEquatable{T}"/> interface. 
    /// </remarks>
    public static bool EqualsEquatable<T> (T a, T b)
      where T : IEquatable<T>
    {
      if (a == null)
        return (b == null);
      else
        return a.Equals ((T) b);
    }

    /// <summary>
    /// Returns whether two objects are equal.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="object.Equals(object,object)"/>, only with less boxing going on (better performance).
    /// </remarks>
    public static bool Equals<T> (T a, T b)
    {
      if (a == null)
        return (b == null);
      else
        return a.Equals ((object) b);
    }
  }
}
