using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.SessionState;
using System.Security.Principal;

using Rubicon.Utilities;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;

using SecurityManagerUser = Rubicon.SecurityManager.Domain.OrganizationalStructure.User;
using System.Threading;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  public class SecurityManagerHttpApplication : HttpApplication
  {
    // constants

    // types

    // static members

    private static readonly string s_clientKey = typeof (SecurityManagerHttpApplication).AssemblyQualifiedName + "_Client";
    private static readonly string s_userKey = typeof (SecurityManagerHttpApplication).AssemblyQualifiedName + "_User";

    // member fields

    // construction and disposing

    public SecurityManagerHttpApplication ()
    {
    }

    // methods and properties

    public void SetCurrentUser (SecurityManagerUser user, bool setCurrentClient)
    {
      IPrincipal principal = GetPrincipal (user);
      HttpContext.Current.User = principal;
      Thread.CurrentPrincipal = principal;
      SaveUserToSession (user, false);
      SecurityManagerUser.Current = user;
      if (setCurrentClient)
      {
        Client client;
        using (new SecurityFreeSection ())
        {
          client = (user != null) ? user.Client : null;
        }
        SetCurrentClient (client);
      }
    }

    protected virtual IPrincipal GetPrincipal (SecurityManagerUser user)
    {
      string userName = (user != null) ? user.UserName : string.Empty;
      return new GenericPrincipal (new GenericIdentity (userName), new string[0]);
    }

    public ObjectID LoadUserIDFromSession ()
    {
      return (ObjectID) Session[s_userKey];
    }

    public SecurityManagerUser LoadUserFromSession (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      ObjectID userID = LoadUserIDFromSession ();
      if (userID == null)
        return null;

      return SecurityManagerUser.GetObject (userID, clientTransaction);
    }

    public void SaveUserToSession (SecurityManagerUser user, bool saveCurrentClient)
    {
      if (user == null)
        Session.Remove (s_userKey);
      else
        Session[s_userKey] = user.ID;

      if (saveCurrentClient)
      {
        Client client;
        using (new SecurityFreeSection ())
        {
          client = (user != null) ? user.Client : null;
        }
        SaveClientToSession (client);
      }
    }

    public void SetCurrentClient (Client client)
    {
      SaveClientToSession (client);
      Client.Current = client;
    }

    public ObjectID LoadClientIDFromSession ()
    {
      return (ObjectID) Session[s_clientKey];
    }

    public Client LoadClientFromSession (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      ObjectID clientID = LoadClientIDFromSession ();
      if (clientID == null)
        return null;

      return Client.GetObject (clientID, clientTransaction);
    }

    public void SaveClientToSession (Client client)
    {
      if (client == null)
        Session.Remove (s_clientKey);
      else
        Session[s_clientKey] = client.ID;
    }

    protected bool HasSessionState
    {
      get { return Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState; }
    }

    public override void Init ()
    {
      base.Init ();

      PostAcquireRequestState += new EventHandler (SecurityManagerHttpApplication_PostAcquireRequestState); 
    }

    private void SecurityManagerHttpApplication_PostAcquireRequestState (object sender, EventArgs e)
    {
      if (HasSessionState)
      {
        ClientTransaction clientTransaction = new ClientTransaction ();
        
        SecurityManagerUser user = LoadUserFromSession (clientTransaction);
        if (user == null && Context.User.Identity.IsAuthenticated)
        {
          user = SecurityManagerUser.FindByUserName (Context.User.Identity.Name, clientTransaction);
          SetCurrentUser (user, true);
        }
        else
        {
          SetCurrentUser (user, false);
          SetCurrentClient (LoadClientFromSession (clientTransaction));
        }
      }
    }
  }
}