<%@ Register TagPrefix="iuc" TagName="IntegrationTestUserControl" Src="IntegrationTestUserControl.ascx" %>
<%@ Page language="c#" Codebehind="IntegrationTestUserControlForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.IntegrationTestUserControlForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Integration Test: User Control Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>IntegrationTest: UserControl Form</h1>
<p><iuc:IntegrationTestUserControl id="IntegrationTestUserControl" runat="server"></iuc:IntegrationTestUserControl></p></form>
  </body>
</html>
