using System;
using System.Web.UI;
using Rubicon.Globalization;
using Rubicon.Collections;

namespace Rubicon.Web.Utilities
{
/// <summary>
///   Functionality for working with <see cref="IResourceManager"/> in Controls.
/// </summary>
public class ResourceManagerUtility
{
  /// <summary>
  ///   Get resource managers of all controls impementing <see cref="IObjectWithResources"/> in the 
  ///   current control's hierarchy (parents last).
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented. If more than
  ///   one resource manager is found, an <see cref="ResourceManagerSet"/> is returned.
  /// </returns>
  public static IResourceManager GetResourceManager (Control control)
  {
    if (control == null)
      return null;

    TypedArrayList resourceManagers = new TypedArrayList (typeof (IResourceManager));

    GetResourceManagersRecursive (control, resourceManagers);

    if (resourceManagers.Count == 0)
      return null;
    else if (resourceManagers.Count == 1)
      return resourceManagers[0];
    else
      return new ResourceManagerSet ((IResourceManger[]) resourceManagers.ToArray());
  }

  private void GetResourceManagersRecursive (Control control, TypedArrayList resourceManagers)
  {
    if (control == null)
      return;

    IObjectWithResources objectWithResources  = control as IObjectWithResources;

    if (objectWithResources != null)
      return resourceManagers.Add (objectWithResources.GetResourceManager());

    GetResourceManagersRecursive (control.Parent, resourceManagers);
  }

  //  No construction for static only class
  /// <exclude />
	private ResourceManagerUtility()
	{
  }
}
}
