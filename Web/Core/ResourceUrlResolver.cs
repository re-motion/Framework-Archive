using System;
using System.Web.UI;
using System.Web;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class ResourceUrlResolver
{
  private const string c_designTimeRootDefault = "C:\\Rubicon.Resources";
  private const string c_designTimeRootEnvironmentVaribaleName = "RUBICONRESOURCES";

  /// <summary>
  ///   Returns the physical URL of a resource item.
  ///   <seealso cref="IResourceUrlResolver"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If the current ASP.NET application object implements <see cref="IResourceUrlResolver"/>, the application 
  ///     object creates the URL string. 
  ///     Otherwise, or if <paramref name="context"/> is <see langword="null"/>, the URL 
  ///     <c>&lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl</c> is used.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration,
  ///     <see cref="Rubicon.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Rubicon.Web/Image/Help.gif</c>.
  ///   </para><para>
  ///     During design time, the <b>resource root</b> is mapped to the environment variable
  ///     <c>RUBICONRESOURCES</c>, or if the variable does not exist, <c>C:\Rubicon.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. Currently, this parameter is only used to detect design time.
  /// </param>
  /// <param name="context"> The current <see cref="HttpContext"/>. </param>
  /// <param name="definingType"> The type that this resource item is associated with. </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) </param>
  /// <param name="relativeUrl"> The relative URL of the item. </param>
  public static string GetResourceUrl (
      Control control, 
      HttpContext context,
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    IResourceUrlResolver resolver = null;
    if (context != null)
      resolver = context.ApplicationInstance as IResourceUrlResolver;
    if (resolver != null)
      return resolver.GetResourceUrl (control, definingType, resourceType, relativeUrl);
    else
      return GetResourceUrl (control, definingType, resourceType, relativeUrl);
  }

  /// <summary>
  ///   Returns the physical URL of a resource item.
  ///   <seealso cref="IResourceUrlResolver"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Uses the URL &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration,
  ///     <see cref="Rubicon.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Rubicon.Web/Image/Help.gif</c>.
  ///   </para><para>
  ///     During design time, the <b>resource root</b> is mapped to the environment variable
  ///     <c>RUBICONRESOURCES</c>, or if the variable does not exist, <c>C:\Rubicon.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. Currently, this parameter is only used to detect design time.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <param name="definingType"> 
  ///   The type that this resource item is associated with. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) Must not be <see langword="null"/>. </param>
  /// <param name="relativeUrl"> The relative URL of the item. Must not be <see langword="null"/> or empty.</param>
  public static string GetResourceUrl (
      Control control, 
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    ArgumentUtility.CheckNotNull ("definingType", definingType);
    ArgumentUtility.CheckNotNull ("resourceType", resourceType);
    ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

    string assemblyName = definingType.Assembly.FullName.Split(',')[0];
    string root;
    if (Rubicon.Web.Utilities.ControlHelper.IsDesignMode (control))
      root = GetDesignTimeRoot();
    else 
      root = GetRunTimeRoot();
    if (! root.EndsWith ("/"))
      root += "/";
    return root + assemblyName + "/" + resourceType.Name + "/" + relativeUrl;
  }

  private static string GetRunTimeRoot()
  {
    string root = Configuration.WebConfiguration.Current.Resources.Root;

    if (Configuration.WebConfiguration.Current.Resources.RelativeToApplicationRoot)
    {
      if (! root.StartsWith ("/"))
        root = "/" + root;
      if (HttpRuntime.AppDomainAppVirtualPath != "/")
        root = HttpRuntime.AppDomainAppVirtualPath + root;
    }
    return root;
  }

  private static string GetDesignTimeRoot()
  { 
    string root = System.Environment.GetEnvironmentVariable (c_designTimeRootEnvironmentVaribaleName);
    if (StringUtility.IsNullOrEmpty (root))
      root = c_designTimeRootDefault;
    return root;

    //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
    //if(environment != null)
    //{
    //  EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;          
    //  //  project.Properties uses a 1-based index
    //  for (int i = 1; i <= project.Properties.Count; i++)
    //  {
    //    if(project.Properties.Item (i).Name == "ActiveFileSharePath")
    //      return project.Properties.Item (i).Value.ToString();
    //  }
    //}
  }
}

}
