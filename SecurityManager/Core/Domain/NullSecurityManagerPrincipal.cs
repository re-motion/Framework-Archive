// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="NullSecurityManagerPrincipal"/> type is the <see cref="INullObject"/> implementation 
  /// of the <see cref="ISecurityManagerPrincipal"/> interface.
  /// </summary>
  [Serializable]
  public sealed class NullSecurityManagerPrincipal:ISecurityManagerPrincipal, IObjectReference
  {
    internal NullSecurityManagerPrincipal ()
    {       
    }

    public Tenant Tenant
    {
      get { return null; }
    }

    public User User
    {
      get { return null; }
    }

    public Substitution Substitution
    {
      get { return null; }
    }

    public Tenant GetTenant (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      return null;
    }

    public User GetUser (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      return null;
    }

    public Substitution GetSubstitution (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      return null;
    }

    public void Refresh ()
    {
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return SecurityManagerPrincipal.Null;
    }
  }
}