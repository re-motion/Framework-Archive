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

    private static Dictionary<Enum, AccessType> s_cache = new Dictionary<Enum, AccessType> ();

    public static AccessType Get (Enum accessType)
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

    private Enum _value;
    private string _id;

    // construction and disposing

    private AccessType (Enum accessType)
    {
      Type type = accessType.GetType ();
      if (!type.IsDefined (typeof (AccessTypeAttribute), false))
      {
        throw new ArgumentException (string.Format ("Enumerated Type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Rubicon.Security.AccessTypeAttribute applied.", type.FullName),
            "accessType");
      }

      _value = accessType;
      _id = type.FullName + "." + accessType.ToString () + ", " + type.Assembly.GetName ().Name;
    }

    // methods and properties

    public string ID
    {
      get { return _id; }
    }

    public Enum Value
    {
      get { return _value; }
    }
  }

}