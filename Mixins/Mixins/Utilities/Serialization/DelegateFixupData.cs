using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public class DelegateFixupData
  {
    private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _typeName;
    private object _target;
    private MethodInfoFixupData _method;

    public DelegateFixupData (Delegate delegateObject)
    {
      ArgumentUtility.CheckNotNull ("delegateObject", delegateObject);

      _typeName = delegateObject.GetType().AssemblyQualifiedName;
      _target = delegateObject.Target;
      _method = new MethodInfoFixupData(delegateObject.Method);
    }

    public Delegate GetDelegate ()
    {
      Type delegateType = Type.GetType (_typeName);
      MethodInfo method = _method.GetMethodInfo();
      return Delegate.CreateDelegate (delegateType, _target, method);
    }

    internal static object PrepareDelegate (object data)
    {
      Delegate delegateObjectInfo = data as Delegate;
      if (delegateObjectInfo == null)
        throw new ArgumentException ("Invalid data object - Delegate expected.", "data");
      return new DelegateFixupData (delegateObjectInfo);
    }

    internal static object FixupDelegate (object data)
    {
      DelegateFixupData fixupData = data as DelegateFixupData;
      if (fixupData == null)
        throw new ArgumentException ("Invalid data object - DelegateFixupData expected.", "data");
      return fixupData.GetDelegate ();
    }

  }
}