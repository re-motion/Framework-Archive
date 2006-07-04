using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Clients.Web.Globalization.Classes;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  public class SecurableClassDefinitionTreeView : BocTreeView
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public SecurableClassDefinitionTreeView ()
    {
    }

    // methods and properties

    protected override string GetText (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      string text = base.GetText (businessObject);

      SecurableClassDefinition classDefinition = businessObject as SecurableClassDefinition;
      if (classDefinition == null)
        return text;

      if (classDefinition.AccessControlLists.Count == 0)
        return string.Format (SecurableClassDefinitionTreeViewResources.NoAclsText, text);
      if (classDefinition.AccessControlLists.Count == 1)
        return string.Format (SecurableClassDefinitionTreeViewResources.SingleAclText, text);
      else
        return string.Format (SecurableClassDefinitionTreeViewResources.MultipleAclsText, text, classDefinition.AccessControlLists.Count);
    }
  }
}