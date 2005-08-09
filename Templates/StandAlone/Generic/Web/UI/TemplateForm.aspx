<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="tabs" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Page language="c#" Codebehind="TemplateForm.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Templates.Generic.Web.UI.TemplateForm" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
  </head>
  <body>
    <form id="Form" method="post" runat="server">
      <rubicon:FormGridManager id="FormGridManager" runat="server"></rubicon:FormGridManager>
      <tabs:NavigationTabs id="NavigationTabs" runat="server"></tabs:NavigationTabs>
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="WIDTH: 100%">
        <tr>
          <td colSpan=2>Heading</td>
        </tr>
        <tr>
          <td><rubicon:SmartLabel id="SmartLabel" runat="server" Text="Label" ForControl="BocTextValue"></rubicon:SmartLabel></td>
          <td><obw:BocTextValue id="BocTextValue" runat="server">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</obw:BocTextValue></td>
        </tr>
      </table>
      <div style="MARGIN-TOP: 1em"><rubicon:WebButton id="SubmitButton" runat="server" Text="Submit" class="Button"></rubicon:WebButton></div>
    </form>
  </body>
</html>
