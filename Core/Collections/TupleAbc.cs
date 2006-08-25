using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Security.Principal;

namespace Rubicon.Collections
{
  // TODO: Doc
  public class Tuple<TA, TB, TC> : IEquatable<Tuple<TA, TB, TC>>
  {
    // types

    // static members

    // member fields

    private TA _a;
    private TB _b;
    private TC _c;

    // construction and disposing

    public Tuple (TA a, TB b, TC c)
    {
      ArgumentUtility.CheckNotNull ("a", a);
      ArgumentUtility.CheckNotNull ("b", b);
      ArgumentUtility.CheckNotNull ("c", c);

      _a = a;
      _b = b;
      _c = c;
    }

    // methods and properties


    public TA A
    {
      get { return _a; }
    }

    public TB B
    {
      get { return _b; }
    }

    public TC C
    {
      get { return _c; }
    }

    public bool Equals (Tuple<TA, TB, TC> other)
    {
      if (other == null)
        return false;

      return this._a.Equals (other._a) && this._b.Equals (other._b) && this._c.Equals (other._c);
    }

    public override bool Equals (object obj)
    {
      Tuple<TA, TB, TC> other = obj as Tuple<TA, TB, TC>;
      if (other == null)
        return false;
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return _a.GetHashCode () ^ _b.GetHashCode () ^ _c.GetHashCode ();
    }
  }
}