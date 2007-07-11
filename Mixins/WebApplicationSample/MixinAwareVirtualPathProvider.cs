using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Hosting;
using System.Collections.Generic;

namespace WebApplicationSample
{
  public class MixinAwareVirtualPathProvider : VirtualPathProvider
  {
    private readonly Dictionary<string, Type> _baseNameToConcreteTypeMap;

    public MixinAwareVirtualPathProvider (Dictionary<string, Type> baseNameToConcreteTypeMap)
    {
      _baseNameToConcreteTypeMap = baseNameToConcreteTypeMap;
    }

    public override VirtualFile GetFile(string virtualPath)
    {
      List<string> pathElements = new List<string> (virtualPath.Split ('/'));

      if (pathElements.Contains("SimpleWebForm.aspx"))
        return new MixinAwareVirtualFile(virtualPath, _baseNameToConcreteTypeMap);

      return base.GetFile(virtualPath);
    }

  }
}
