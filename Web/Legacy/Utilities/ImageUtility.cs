using System;
using System.Web.UI;

using Rubicon.Globalization;


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
        ResourceManagerPool.GetResourceText (typeof (UIUtility), "RequiredFieldText"), 
        "~/images/field-required.gif");
  }

  internal static string GetIconImage (Page page, string tooltip, string imagePath)
  {
    return string.Format (
        "<img src=\"{0}\" alt=\"{1}\" width=\"12\" height=\"20\" border=\"0\"/>", 
        page.ResolveUrl (imagePath), tooltip);
  }
}
}
