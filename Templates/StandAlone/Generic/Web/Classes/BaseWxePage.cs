using System;
using System.Collections;
using System.Web.UI;

using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;

using Rubicon.Templates.Generic.Web.WxeFunctions;

namespace Rubicon.Templates.Generic.Web.Classes
{

public class BaseWxePage : WxePage, IObjectWithResources
{
  public BaseFunction CurrentBaseFunction 
  {
    get { return CurrentFunction as BaseFunction; }
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    string key = GetType().FullName + "_RubiconStyle";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (ResourceUrlResolver), ResourceType.Html, "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    key = GetType().FullName + "_GlobalStyle";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, null, ResourceType.Html, "Global.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this);
    if (resourceManager != null)
      ResourceDispatcher.Dispatch (this, resourceManager);
  }

  /// <summary> Gets the <see cref="IResourceManager"/> for the page. </summary>
  /// <returns> 
  ///   An <see cref="IResourceManager"/> containing all resources defined for this page and it's descendents
  ///   or <see langword="null"/> if no resources have been defined.
  /// </returns>
  /// <remarks> 
  ///   <para>
  ///     Use <see cref="ResourceManagerUtility.GetResourceManager"/> to access the page's resources.
  ///   </para><para>
  ///     The default implementation uses the <see cref="MultiLingualResourcesAttribute"/> to get the resource manager.
  ///   </para>
  ///   <note type="inheritinfo">
  ///     Specify a <see cref="MultiLingualResourcesAttribute"/> for the class definition of the specialized class.
  ///     If no attribute is specified, <b>GetResourceManager</b> will return <see langword="null"/>.
  ///   </note>
  /// </remarks>
  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResourcesAttribute.ExistsResource (type))
      return MultiLingualResourcesAttribute.GetResourceManager (type, true);
    else
      return null;
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }
}

}
