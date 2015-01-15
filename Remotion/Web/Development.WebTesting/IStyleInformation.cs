using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Provides assertable style information for a <see cref="ControlObject"/>.
  /// </summary>
  public interface IStyleInformation
  {
    /// <summary>
    /// Returns whether the control bears the given <paramref name="cssClass"/>.
    /// </summary>
    /// <returns><see langword="true" /> if the control has the given <paramref name="cssClass"/>, otherwise <see langword="false" />.</returns>
    bool HasCssClass ([NotNull] string cssClass);

    /// <summary>
    /// Returns the computed background color of the control. This method ignores background images as well as transparencies - the first
    /// non-transparent color set in the node's DOM hierarchy is returned.
    /// </summary>
    /// <returns>The background color or <see cref="Color.Transparent"/> if no background color is set (not even on any parent node).</returns>
    Color GetBackgroundColor ();

    /// <summary>
    /// Returns the computed text color of the control. This method ignores transparencies - the first non-transparent color set in the node's
    /// DOM hierarchy is returned.
    /// </summary>
    /// <returns>The text color or <see cref="Color.Transparent"/> if no text color is set (not even on any parent node).</returns>
    Color GetTextColor ();
  }
}