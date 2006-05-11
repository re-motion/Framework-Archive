using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{

  public class AccessTypeReflector : Rubicon.Security.Metadata.IAccessTypeReflector
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public AccessTypeReflector ()
    {
    }

    // methods and properties

    public List<EnumValueInfo> GetAccessTypes (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("cache", cache);

      //return new EnumerationReflector ().GetValues (typeof (GeneralAccessType));
      return new List<EnumValueInfo>();
    }
  }
}