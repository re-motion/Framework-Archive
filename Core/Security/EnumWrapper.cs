using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public struct EnumWrapper
  {
    private readonly string _typeName;
    private readonly string _name;
    private Enum _enumValue;

    public EnumWrapper (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      Type type = enumValue.GetType ();
      if (Attribute.IsDefined (type, typeof (FlagsAttribute), false))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' cannot be wrapped. Only enumerated types without the {1} can be wrapped.", 
                type.FullName, 
                typeof (FlagsAttribute).FullName),
            "enumValue");
      }
      
      _typeName = TypeUtility.GetPartialAssemblyQualifiedName (type);
      _name = enumValue.ToString ();
      _enumValue = enumValue;
    }

    public EnumWrapper (string typeName, string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _typeName = TypeUtility.ParseAbbreviatedTypeName (typeName);
      _name = name;
      _enumValue = null;
    }

    public string TypeName
    {
      get { return _typeName; }
    }	

    public string Name
    {
      get { return _name; }
    }

    /// <summary> Returns the enum value wrapped by this <see cref="EnumWrapper"/>. </summary>
    /// <returns> The enum value designated by the <see cref="TypeName"/> and <see cref="Name"/> properties. </returns>
    /// <exception cref="TypeLoadException"> The <see cref="TypeName"/> cannot by found. </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="TypeName"/> is not an enumerated type. 
    ///   <br/>- or -<br/>
    ///   The <see cref="Name"/> does not designate a valid value of the enumerated type.
    /// </exception>
    public Enum GetEnum ()
    {
      if (_enumValue == null)
      {
        Type type = Type.GetType (_typeName, true, false);
        
        if (!type.IsEnum)
          throw new InvalidOperationException (string.Format ("The type '{0}' is not an enumerated type.", _typeName));
        if (!Enum.IsDefined (type, _name))
          throw new InvalidOperationException (string.Format ("The enumerated type '{0}' does not define the value '{1}'.", _typeName, _name));
          
        _enumValue = (Enum) Enum.Parse (type, _name, false);
      }

      return _enumValue;
    }

    /// <summary> Compares the supplied object parameter to the <see cref="TypeName"/> and  <see cref="Name"/> properties. </summary>
    /// <param name="obj"> The object to be compared. </param>
    /// <returns>
    /// <see langword="true"/> if object is an instance of <see cref="EnumWrapper"/> and the two are equal; otherwise <see langword="false"/>.
    /// If object is a null reference, <see langword="false"/> is returned.
    /// </returns>
    public override bool Equals (object obj)
    {
      if (!(obj is EnumWrapper))
        return false;

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
      return this._name == value._name && String.Equals (this._typeName, value._typeName, StringComparison.Ordinal);
    }

    public override int GetHashCode ()
    {
      return _name.GetHashCode () ^ _typeName.GetHashCode ();
    }
  }

}