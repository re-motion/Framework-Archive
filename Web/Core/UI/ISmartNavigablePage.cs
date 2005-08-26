using System;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI
{

public interface ISmartNavigablePage: IPage
{
  /// <summary> Gets or sets the flag that determines whether smart scrolling is enabled on this page.  </summary>
  /// <value> <see langowrd="true"/> to enable smart scrolling. </value>
  bool IsSmartScrollingEnabled { get; }

  /// <summary> Gets or sets the flag that determines whether smart naviagtion is enabled on this page.  </summary>
  /// <value> <see langowrd="true"/> to enable smart focusing. </value>
  bool IsSmartFocusingEnabled { get; }

  /// <summary> Clears scrolling and focus information on the page. </summary>
  void DiscardSmartNavigationData ();

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> The <see cref="IFocusableControl"/> to assign the focus to. </param>
  /// <remarks> In dotNet 2.0, the focus can be set even if smart focusing is disabled. </remarks>
  void SetFocus (IFocusableControl control);

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> The client side ID of the control to assign the focus to. </param>
  /// <remarks> Must be called before PreRendering of the page it self (the last control in the PreRender Phase). </remarks>
  void SetFocus (string id);
}

}
