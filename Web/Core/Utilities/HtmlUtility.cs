using System;
using System.Web;

namespace Rubicon.Web.UI.Controls
{
public class HtmlUtility
{
  public static string Format (string htmlFormatString, params object[] nonHtmlParameters)
  {
    string[] htmlParameters = new string[nonHtmlParameters.Length];
    for (int i = 0; i < nonHtmlParameters.Length; ++i)
    {
      htmlParameters[i] = HtmlEncode (nonHtmlParameters[i].ToString());
    }
    return string.Format (htmlFormatString, (object[]) htmlParameters);
  }

  public static string HtmlEncode (string nonHtmlString)
  {
    string html = HttpUtility.HtmlEncode (nonHtmlString);
    html = html.Replace ("\r\n", "<br>");
    html = html.Replace ("\n", "<br>");
    html = html.Replace ("\r", "<br>");
    return html;
  }

  private HtmlUtility()
  {
  }
}
}
