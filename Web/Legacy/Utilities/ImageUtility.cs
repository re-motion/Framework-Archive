using System;
using System.IO;
using System.Web.UI;

using Rubicon.Globalization;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Utilities
{

internal sealed class UIUtility
{
	private UIUtility()
	{
	}

  internal static string GetErrorImage (Page page, string errorMessage)
  {
    return GetIconImage (page, errorMessage, "~/images/field-error.gif");
  }

  internal static string GetRequiredFieldImage (Page page)
  {
    return GetIconImage (
        page,
        // TODO: Check if change works: Changed to use MultiLingualResourcesAttribute instead of ResourceManagerPool
        //  ResourceManagerPool.GetResourceText (typeof (UIUtility), "RequiredFieldText"), 
        MultiLingualResourcesAttribute.GetResourceText (typeof (UIUtility), "RequiredFieldText"), 
        "~/images/field-required.gif");
  }

  internal static string GetIconImage (Page page, string tooltip, string imagePath)
  {
    return string.Format (
        "<img src=\"{0}\" alt=\"{1}\" width=\"12\" height=\"20\" border=\"0\"/>", 
        page.ResolveUrl (imagePath), tooltip);
  }

  internal static string GetWhitespaceImage (string imagePath, int width, int height)
  {
    // Specify at least an empty alt text to be HTML 4.0 conform (eGov Gütesiegel)
    return string.Format ("<img border=\"0\" width=\"{0}\" height=\"{1}\" src=\"{2}\" alt=\"\">", 
      width, height, GetImagePath (imagePath, "ws.gif"));
  }

  internal static string GetWhitespaceImage (string imagePath, string width, string height)
  {
    // Specify at least an empty alt text to be HTML 4.0 conform (eGov Gütesiegel)
    return string.Format ("<img border=\"0\" width=\"{0}\" height=\"{1}\" src=\"{2}\" alt=\"\">", 
      width, height, GetImagePath (imagePath, "ws.gif"));
  }

  internal static string GetImagePath (string imagePath, string imageFileName)
  {
    return UrlUtility.Combine (imagePath, imageFileName);
  }

  /// <summary>
  /// Returns the image path from a parental EntryFormGrid or ViewControl.
  /// </summary>
  /// <param name="control">The control for which the image path should be read.</param>
  /// <returns>The image path.</returns>
  internal static string GetImagePathFromParentControl (Control control)
  {
    if (control != null)
    {
      EntryFormGrid entryFormGrid = control as EntryFormGrid;
      if (entryFormGrid != null)
      {
        return entryFormGrid.ImagePath;
      }
      else
      {
        ViewControl viewControl = control as ViewControl;
        if (viewControl != null)
          return viewControl.ImagePath;
        else
          return GetImagePathFromParentControl (control.Parent);
      }
    }
    else
    {
      throw new InvalidOperationException (
          "Image path could not be read from parent control. "
          + "Control must be placed inside a EntryFormGrid or a ViewControl.");
    }
  }
}
}
