using System;
using System.Web.UI;

using Rubicon.Globalization;

namespace Rubicon.Web.UI.Utilities
{
/// <summary>
/// Summary description for ResourceManagerUtility.
/// </summary>
public class ResourceManagerUtility
{
  public static IResourceManager GetResourceManager (Control control)
  {
    IObjectWithResources objectWithResources  = control as IObjectWithResources;

    if (objectWithResources != null)
      return objectWithResources.GetResourceManager();

    if (control.Parent == null)
      return null;

    return GetResourceManager (control.Parent);
  }

  //  No construction for static only class
	private ResourceManagerUtility()
	{}
}
}
