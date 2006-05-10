using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{

  public sealed class AccessType
  {
    // types

    // static members and constants

    private static Dictionary<EnumWrapper, AccessType> s_cache = new Dictionary<EnumWrapper, AccessType> ();

    public static AccessType Get (Enum accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      return Get (new EnumWrapper (accessType));
    }

    public static AccessType Get (EnumWrapper accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      if (s_cache.ContainsKey (accessType))
        return s_cache[accessType];

      lock (s_cache)
      {
        if (s_cache.ContainsKey (accessType))
          return s_cache[accessType];

        AccessType temp = new AccessType (accessType);
        s_cache.Add (accessType, temp);
        return temp;
      }
    }

    // member fields

    private EnumWrapper _value;

    // construction and disposing

    private AccessType (EnumWrapper accessType)
    {
      Type type = TypeUtility.GetType (accessType.TypeName);
      if (!type.IsDefined (typeof (AccessTypeAttribute), false))
      {
        throw new ArgumentException (string.Format ("Enumerated type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Rubicon.Security.AccessTypeAttribute applied.", type.FullName),
            "accessType");
      }

      _value = accessType;
    }

    // methods and properties

    public EnumWrapper Value
    {
      get { return _value; }
    }
  }

}