<%@ Page language="c#" Codebehind="Form.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.IndividualControlTests.IndividualControlTestForm" %>
<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <title>IndividualControlTestForm</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <rubicon:htmlheadcontents id=HtmlHeadContents runat="server"/>
  </head>
  <body >
	
    <form id="Form" method="post" runat="server">
<obwt:NavigationTabs id="NavigationTabs" runat="server"/>
<div>
<rubicon:webbutton id=PostBackButton runat="server" Text="Post Back"/>
<rubicon:webbutton id=SaveButton runat="server" Width="120px" Text="Save"/>
<rubicon:webbutton id="SaveAndRestartButton" runat="server" Text="Save &amp; Restart" Width="120px"/>
<rubicon:webbutton id="CancelButton" runat="server" Text="Cancel" Width="120px"/>
</div>
<div><asp:placeholder id="UserControlPlaceHolder" runat="server"/></div>
<div><asp:literal id="Stack" runat="server"/></div>
<div 
style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls"><obr:reflectionbusinessobjectdatasourcecontrol 
id=CurrentObject runat="server" 
typename="OBRTest.Person, OBRTest"/></div>
</form>
	
  </body>
</html>
