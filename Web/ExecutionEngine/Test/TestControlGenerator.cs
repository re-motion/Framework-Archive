using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.PageTransition
{
  public class TestControlGenerator
  {
    public event EventHandler<IDEventArgs> Click;
    private readonly Page _page;

    public TestControlGenerator (Page page)
    {
      _page = page;
    }

    public IEnumerable GetTestControls (string prefix)
    {
      yield return CreateInputControlWithSubmitBehavior (prefix);
      yield return CreateInputControlWithButtonBehavior (prefix);
      yield return CreateButtonControlWithSubmitBehavior (prefix);
      yield return CreateButtonControlWithButtonBehavior (prefix);

      yield return CreateAnchorWithTextAndJavascriptInOnClick (prefix);
      yield return CreateAnchorWithTextAndJavascriptInHref (prefix);
      yield return CreateAnchorWithImageAndJavascriptInOnClick (prefix);
      yield return CreateAnchorWithImageAndJavascriptInHref (prefix);
      yield return CreateAnchorWithSpanAndJavascriptInOnClick (prefix);
      yield return CreateAnchorWithSpanAndJavascriptInHref (prefix);
      yield return CreateAnchorWithLabelAndJavascriptInOnClick (prefix);
      yield return CreateAnchorWithLabelAndJavascriptInHref (prefix);
      yield return CreateAnchorWithBoldAndJavascriptInOnClick (prefix);
      yield return CreateAnchorWithBoldAndJavascriptInHref (prefix);
    }

    private void OnClick (object sender, EventArgs e)
    {
      Control control = (Control) sender;
      if (Click != null)
        Click (this, new IDEventArgs (control.ID));
    }

    private Control CreateInputControlWithSubmitBehavior (string prefix)
    {
      Button button = new Button();
      button.ID = CreateTestMatrixControlID (prefix, "InputSubmit");
      button.Text = "Submit";
      button.UseSubmitBehavior = true;
      button.Click += OnClick;

      return button;
    }

    private Control CreateInputControlWithButtonBehavior (string prefix)
    {
      Button button = new Button();
      button.ID = CreateTestMatrixControlID (prefix, "InputButton");
      button.Text = "Button";
      button.UseSubmitBehavior = false;
      button.Click += OnClick;

      return button;
    }

    private Control CreateButtonControlWithSubmitBehavior (string prefix)
    {
      WebButton button = new WebButton();
      button.ID = CreateTestMatrixControlID (prefix, "ButtonSubmit");
      button.Text = "Submit";
      button.UseSubmitBehavior = true;
      button.Click += OnClick;

      return button;
    }

    private Control CreateButtonControlWithButtonBehavior (string prefix)
    {
      WebButton button = new WebButton();
      button.ID = CreateTestMatrixControlID (prefix, "ButtonButton");
      button.Text = "Button";
      button.UseSubmitBehavior = false;
      button.Click += OnClick;

      return button;
    }
    
    private Control CreateAnchorWithTextAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithJavascriptInOnClick (prefix, "AnchorWithTextAndJavascriptInOnClick");
      hyperLink.Text = "OnClick";

      return hyperLink;
    }

    private Control CreateAnchorWithTextAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithJavascriptInHref (prefix, "AnchorWithTextAndJavascriptInHref");
      linkButton.Text = "Href";

      return linkButton;
    }

    private Control CreateAnchorWithImageAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithJavascriptInOnClick (prefix, "AnchorWithImageAndJavascriptInOnClick");
      hyperLink.Controls.Add (CreateImage (hyperLink.ID, "OnClick"));

      return hyperLink;
    }

    private Control CreateAnchorWithImageAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithJavascriptInHref (prefix, "AnchorWithImageAndJavascriptInHref");
      linkButton.Controls.Add (CreateImage (linkButton.ID, "Href"));

      return linkButton;
    }

    private Control CreateAnchorWithSpanAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithJavascriptInOnClick (prefix, "AnchorWithSpanAndJavascriptInOnClick");
      hyperLink.Controls.Add (CreateHtmlGenericControl (hyperLink.ID, "OnClick", "span"));

      return hyperLink;
    }

    private Control CreateAnchorWithSpanAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithJavascriptInHref (prefix, "AnchorWithSpanAndJavascriptInHref");
      linkButton.Controls.Add (CreateHtmlGenericControl (linkButton.ID, "Href", "span"));

      return linkButton;
    }

    private Control CreateAnchorWithLabelAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithJavascriptInOnClick (prefix, "AnchorWithLabelAndJavascriptInOnClick");
      hyperLink.Controls.Add (CreateHtmlGenericControl (hyperLink.ID, "OnClick", "label"));

      return hyperLink;
    }

    private Control CreateAnchorWithLabelAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithJavascriptInHref (prefix, "AnchorWithLabelAndJavascriptInHref");
      linkButton.Controls.Add (CreateHtmlGenericControl (linkButton.ID, "Href", "label"));

      return linkButton;
    }

    private Control CreateAnchorWithBoldAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithJavascriptInOnClick (prefix, "AnchorWithBoldAndJavascriptInOnClick");
      hyperLink.Controls.Add (CreateHtmlGenericControl (hyperLink.ID, "OnClick", "b"));

      return hyperLink;
    }

    private Control CreateAnchorWithBoldAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithJavascriptInHref (prefix, "AnchorWithBoldAndJavascriptInHref");
      linkButton.Controls.Add (CreateHtmlGenericControl (linkButton.ID, "Href", "b"));

      return linkButton;
    }

    private Control CreateImage (string prefix, string text)
    {
      Image image = new Image ();
      image.ID = CreateTestMatrixControlID (prefix, "Inner");
      image.AlternateText = text;

      return image;
    }

    private Control CreateHtmlGenericControl (string prefix, string text, string tag)
    {
      HtmlGenericControl span = new HtmlGenericControl (tag);
      span.ID = CreateTestMatrixControlID (prefix, "Inner");
      span.InnerText = text;

      return span;
    }

    private HyperLink CreateAnchorWithJavascriptInOnClick (string prefix, string id)
    {
      HyperLink hyperLink = new HyperLink();
      hyperLink.ID = CreateTestMatrixControlID (prefix, id);
      hyperLink.NavigateUrl = "#";
      hyperLink.Attributes["onclick"] = _page.ClientScript.GetPostBackEventReference (_page, hyperLink.ID);

      return hyperLink;
    }

    private LinkButton CreateAnchorWithJavascriptInHref (string prefix, string id)
    {
      LinkButton linkButton = new LinkButton();
      linkButton.ID = CreateTestMatrixControlID (prefix, id);
      linkButton.Click += OnClick;

      return linkButton;
    }

    private string CreateTestMatrixControlID (string prefix, string id)
    {
      return (string.IsNullOrEmpty (prefix) ? string.Empty : StringUtility.NullToEmpty (prefix) + "_") + id;
    }
  }
}