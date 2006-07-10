using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Collections
{

/// <summary>
///   A hashtable that automatically creates new value objects when queried for a specific key.
/// </summary>
/// <remarks>
///		This collection cannot be modified using <see cref="Add"/> and setting values through the indexer. Getting values through the indexer
///		will assign a new object to the specified key if none exists.
/// </remarks>
public class AutoInitHashtable: Hashtable
{
  public delegate object CreateMethod ();

  private Type _valueType;
  private CreateMethod _createMethod;

  public AutoInitHashtable (Type valueType)
  {
    ArgumentUtility.CheckNotNull ("valueType", valueType);
    _valueType = valueType;
    _createMethod = null;
  }

  public AutoInitHashtable (CreateMethod createMethod)
  {
    ArgumentUtility.CheckNotNull ("createMethod", createMethod);
    _createMethod = createMethod;
    _valueType = null;
  }

  private object CreateObject ()
  {
    if (_valueType != null)
      return Activator.CreateInstance (_valueType);
    else
      return _createMethod();
  }

  public override object this[object key]
  {
    get
    {
      object obj = base[key];
      if (obj == null)
      {
        obj = CreateObject();
        base[key] = obj;
      }
      return obj;
    }

    set
    {
      throw new NotSupportedException ();
    }
  }

#pragma warning disable 809 // specifying obsolete for overridden methods causes a warning, but this is intended here.
  [Obsolete ("Explicitly adding or setting keys or values is not supported.")]
  public override void Add(object key, object value)
  {
    throw new NotSupportedException();
  }
#pragma warning restore 809
}

}
