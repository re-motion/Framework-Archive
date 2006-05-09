using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public struct EnumWrapper
  {
    private readonly string _typeName;
    private readonly int _value;

    public EnumWrapper (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      Type type = value.GetType ();
      if (type.IsDefined (typeof (FlagsAttribute), false))
      {
        throw new ArgumentException (
            string.Format ("Enumerated Type '{0}' cannot be wrapped. Only enumerated types without the FlagsAttribute can be wrapped.", type.FullName),
            "value");
      }
      
      _typeName = TypeUtility.GetPartialAssemblyQualifiedName (type);
      _value = (int) (object) value;      
    }

    public string TypeName
    {
      get { return _typeName; }
    }	

    public int Value
    {
      get { return _value; }
    }

    /// <summary>
    ///   Compares the supplied object parameter to the <see cref="Value"/> and <see cref="TypeName"/> property of the <see cref="EnumWrapper"/> 
    ///   object.
    /// </summary>
    /// <param name="obj"> The object to be compared. </param>
    /// <returns>
    /// <see langword="true"/> if object is an instance of <see cref="EnumWrapper"/> and the two are equal; otherwise <see langword="false"/>.
    /// If object is a null reference, <see langword="false"/> is returned.
    /// </returns>
    public override bool Equals (object obj)
    {
      if (!(obj is EnumWrapper))
        return false; // obj is a null reference or another type then NaBoolean

      return Equals ((EnumWrapper) obj);
    }

    /// <summary>
    ///   Compares the supplied object parameter to the <see cref="Value"/> and <see cref="TypeName"/> property of the <see cref="EnumWrapper"/> 
    ///   object.
    /// </summary>
    /// <param name="value"> The <see cref="EnumWrapper"/> instance to be compared. </param>
    /// <returns>
    /// <see langword="true"/> if the two are equal; otherwise <see langword="false"/>.
    /// </returns>
    public bool Equals (EnumWrapper value)
    {
      return this._value == value._value && String.Equals (this._typeName, value._typeName, StringComparison.Ordinal);
    }

    public override int GetHashCode ()
    {
      return _value.GetHashCode () ^ _typeName.GetHashCode ();
    }
  }

}