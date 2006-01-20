<%@ Register TagPrefix="iuc" TagName="CompleteBocTestUserControl" Src="CompleteBocTestUserControl.ascx" %>
<%@ Page language="c#" Codebehind="CompleteBocTestUserControlForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocUserControlForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>CompleteBocTest: UserControl Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: UserControl Form</h1>
<p><iuc:CompleteBocTestUserControl id="CompleteBocTestUserControl" runat="server"></iuc:CompleteBocTestUserControl></p>
<p><rwc:ValidationStateViewer id="ValidationStateViewer1" runat="server" noticetext="Es sind fehlerhafte Eingaben gefunden worden." visible="true"></rwc:ValidationStateViewer></p></form>
  </body>
</html>
