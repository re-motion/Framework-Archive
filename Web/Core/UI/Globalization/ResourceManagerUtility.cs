using System;
using System.Web.UI;
using Rubicon.Globalization;

namespace Rubicon.Web.Utilities
{
/// <summary>
///   Functionality for working with <see cref="IResourceManager"/> in Controls.
/// </summary>
public class ResourceManagerUtility
{
  /// <summary>
  ///   Find the closest parent <see cref="Control"/> impementing
  ///   <see cref="IObjectWithResources"/>.
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented.
  /// </returns>
  public static IResourceManager GetResourceManager (Control control)
  {
    if (control == null)
      return null;

    IObjectWithResources objectWithResources  = control as IObjectWithResources;

    if (objectWithResources != null)
      return objectWithResources.GetResourceManager();

    return GetResourceManager (control.Parent);
  }

  //  No construction for static only class
  /// <exclude />
	private ResourceManagerUtility()
	{
  }
}
}
