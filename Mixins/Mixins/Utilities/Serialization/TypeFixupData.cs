using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Utilities.Serialization
{
  [Serializable]
  public class TypeFixupData
  {
    private const BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _typeName;

    public TypeFixupData (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _typeName = type.AssemblyQualifiedName;
    }

    public Type GetTypeObject ()
    {
      return Type.GetType (_typeName);
    }

    internal static object PrepareType (object data)
    {
      Type type = data as Type;
      if (type == null)
        throw new ArgumentException ("Invalid data object - Type expected.", "data");
      return new TypeFixupData (type);
    }

    internal static object FixupType (object data)
    {
      TypeFixupData fixupData = data as TypeFixupData;
      if (fixupData == null)
        throw new ArgumentException ("Invalid data object - TypeFixupData expected.", "data");
      return fixupData.GetTypeObject();
    }

  }
}