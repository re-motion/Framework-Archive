using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Security.Principal;

namespace Rubicon.Security
{
  public class GlobalAccessTypeCacheKey : IEquatable<GlobalAccessTypeCacheKey>
  {
    // types

    // static members

    // member fields

    private SecurityContext _context;
    private string _userName;

    // construction and disposing

    public GlobalAccessTypeCacheKey (SecurityContext context, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("user.Identity", user.Identity);
      ArgumentUtility.CheckNotNullOrEmpty ("user.Identity.Name", user.Identity.Name);

      _context = context;
      _userName = user.Identity.Name;
    }

    public GlobalAccessTypeCacheKey (SecurityContext context, string userName)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      _context = context;
      _userName = userName;
    }

    // methods and properties


    public SecurityContext Context
    {
      get { return _context; }
    }

    public string UserName
    {
      get { return _userName; }
    }

    public bool Equals (GlobalAccessTypeCacheKey other)
    {
      if (other == null)
        return false;

      return this._userName.Equals (other._userName, StringComparison.CurrentCulture) && this._context.Equals (other._context);
    }

    public override bool Equals (object obj)
    {
      GlobalAccessTypeCacheKey other = obj as GlobalAccessTypeCacheKey;
      if (other == null)
        return false;
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return _context.GetHashCode () ^ _userName.GetHashCode ();
    }
  }
}