<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="tabs" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Page language="c#" Codebehind="TemplateForm.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Templates.Generic.Web.UI.TemplateForm" %>
<%@ Register TagPrefix="obw" Namespace="Remotion.ObjectBinding.Web.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
  </head>
  <body>
    <form id="Form" method="post" runat="server">
      <remotion:FormGridManager id="FormGridManager" runat="server"></remotion:FormGridManager>
      <tabs:NavigationTabs id="NavigationTabs" runat="server"></tabs:NavigationTabs>
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="WIDTH: 100%">
        <tr>
          <td colSpan=2>Heading</td>
        </tr>
        <tr>
          <td><remotion:SmartLabel id="SmartLabel" runat="server" Text="Label" ForControl="BocTextValue"></remotion:SmartLabel></td>
          <td><obw:BocTextValue id="BocTextValue" runat="server">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</obw:BocTextValue></td>
        </tr>
      </table>
      <div style="MARGIN-TOP: 1em"><remotion:WebButton id="SubmitButton" runat="server" Text="Submit" class="Button"></remotion:WebButton></div>
    </form>
  </body>
</html>
