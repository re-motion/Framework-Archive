using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Collections
{

/// <summary>
/// A list of objects of a certain type.
/// </summary>
public class TypedArrayList: ArrayList
{
  private Type _elementType;

	public TypedArrayList (Type elementType)
	{
    _elementType = elementType;
	}

  public override int Add (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
    return base.Add (value);
  }

  public override void AddRange (ICollection c)
  {
    ArgumentUtility.CheckNotNull ("c", c);
    ArgumentUtility.CheckItemsNotNullAndType ("c", c, _elementType);
    base.AddRange (c);
  }

  public override void Insert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
    base.Insert (index, value);
  }

  public override void SetRange (int index, ICollection c)
  {
    ArgumentUtility.CheckItemsNotNullAndType ("c", c, _elementType);
    base.SetRange (index, c);
  }

  public override object this[int index]
  {
    get { return base[index]; }
    set
    {
      ArgumentUtility.CheckNotNullAndType ("value", value, _elementType);
      base[index] = value;
    }
  }

  /// <summary>
  ///   Copies the elements of the TypedArrayList to an array of the specified type.
  /// </summary>
  /// <returns></returns>
  public new Array ToArray()
  {
    return base.ToArray (_elementType);
  }
}

}
