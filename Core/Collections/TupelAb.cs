using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Security.Principal;

namespace Rubicon.Collections
{
  // TODO: Doc
  public class Tupel<TA, TB> : IEquatable<Tupel<TA, TB>>
  {
    // types

    // static members

    // member fields

    private TA _a;
    private TB _b;

    // construction and disposing

    public Tupel (TA a, TB b)
    {
      ArgumentUtility.CheckNotNull ("a", a);
      ArgumentUtility.CheckNotNull ("b", b);

      _a = a;
      _b = b;
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

    public bool Equals (Tupel<TA, TB> other)
    {
      if (other == null)
        return false;
      
      return this._a.Equals (other._a) && this._b.Equals (other._b);
    }

    public override bool Equals (object obj)
    {
      Tupel<TA, TB> other = obj as Tupel<TA, TB>;
      if (other == null)
        return false;
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return _a.GetHashCode () ^ _b.GetHashCode ();
    }
  }
}