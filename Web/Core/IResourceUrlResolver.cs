using System;
using System.Web.UI;
using System.Web;
using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
///   Resolve the relative image URL into an absolute image url.
/// </summary>
public interface IImageUrlResolver
{
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/summary' />
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/Parameters/*' />
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/returns' />
	string GetImageUrl (string relativeUrl);
}

/// <summary>
///   Resolve the relative help URL into an absolute help url.
/// </summary>
public interface IHelpUrlResolver
{
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/summary' />
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/Parameters/*' />
  /// <include file='doc\include\IUrlResolver.xml' path='/IUrlResolver/Common/returns' />
	string GetHelpUrl (string relativeUrl);
}

}
