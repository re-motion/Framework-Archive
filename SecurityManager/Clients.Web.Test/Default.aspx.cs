using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Security;
using Rubicon.Security.Configuration;
using Rubicon.SecurityManager.Clients.Web.Test.Domain;
using System.Collections.Generic;
using System.Security.Principal;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load (object sender, EventArgs e)
    {

    }

    protected void EvaluateSecurity_Click (object sender, EventArgs e)
    {
      ISecurityService service = SecurityConfiguration.Current.SecurityService;
      SecurityContext context = new SecurityContext (typeof (File), "1A", "{00000004-1000-0000-0000-000000000007}", "", new Dictionary<string, Enum> (), new Enum[] { DomainAbstractRole.Creator });
      GenericPrincipal user = new GenericPrincipal(new GenericIdentity ("1A"), new string[0]);
      AccessType[] accessTypes = service.GetAccess (context, user);
    }
  }
}
