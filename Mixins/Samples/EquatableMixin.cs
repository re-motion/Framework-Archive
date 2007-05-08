using System;
using Mixins;
using System.Reflection;
using Rubicon.Utilities;

namespace Samples
{
  public class EquatableMixin<T> : Mixin<T>, IEquatable<T>
     where T : class
  {
    private FieldInfo[] _targetFields;

    protected override void OnInitialized ()
    {
      base.OnInitialized ();
      _targetFields = typeof (T).GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    bool IEquatable<T>.Equals (T other)
    {
      if (other == null)
        return false;

      foreach (FieldInfo field in _targetFields)
      {
        object thisFieldValue = field.GetValue (This);
        object otherFieldValue = field.GetValue (other);
        if (!object.Equals (thisFieldValue, otherFieldValue))
          return false;
      }
      return true;
    }

    [Override]
    public new bool Equals (object other)
    {
      return ((IEquatable<T>)this).Equals (other as T);
    }

    [Override]
    public new int GetHashCode ()
    {
      object[] fieldValues = new object[_targetFields.Length];
      for (int i = 0; i < fieldValues.Length; ++i)
        fieldValues[i] = _targetFields[i].GetValue (This);
      
      return EqualityUtility.GetRotatedHashCode (fieldValues);
    }
  }
}
