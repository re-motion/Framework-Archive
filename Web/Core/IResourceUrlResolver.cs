using System;
using System.Web.UI;
using System.Web;
using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI
{
/// <summary>
///   Resolve the relative image URL into an absolute image url.
/// </summary>
public interface IResourceUrlResolver
{
  /// <summary>
  ///   Resolves a relative URL into an absolute URL.
  /// </summary>
  /// <param name="relativeUrl">
  ///   The relative URL to be resolved into an absolute URL.
  /// </param>
  /// <param name="definingType"> 
  ///   The type that defines the resource. If the resource instance is not defined by a type, 
  ///   this is null. 
  /// </param>
  /// <param name="resourceType">
  ///   The type of resource to get. 
  /// </param>
  /// <returns>
  ///   The absulute URL.
  /// </returns>
	string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl);
}

public class ResourceType
{
  public static readonly ResourceType Image = new ResourceType ("Image");
  public static readonly ResourceType Html = new ResourceType ("Html");
  public static readonly ResourceType HelpPage = new ResourceType ("HelpPage");

  private string _name;

  public ResourceType (string name)
  {
    _name = name;
  }

  public string Name
  { 
    get { return _name; }
  }
}

}
