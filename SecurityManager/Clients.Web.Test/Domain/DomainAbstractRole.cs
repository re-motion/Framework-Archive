using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Rubicon.Utilities;
using Rubicon.Data;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  [AbstractRole]
	public enum DomainAbstractRole
	{
    [PermanentGuid ("448355F7-FD2F-41b0-9871-EE3075A4FF73")]
    Creator = 0
	}
}