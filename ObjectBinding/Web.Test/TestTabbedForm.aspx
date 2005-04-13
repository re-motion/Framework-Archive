<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head><title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<table style="height:100%; width:100%;" cellspacing=0 cellpadding=0 border=0>
<tr>
<td style="height:0%;">
<h1>Test Tabbed Form</h1>
<rwc:validationstateviewer id=ValidationStateViewer runat="server" visible="true"></rwc:validationstateviewer>
<rwc:webtabstrip id=PagesTabStrip runat="server" width="100%"></rwc:webtabstrip></td>
</tr>
<tr>
<td style="border: solid 1px red; padding:10px;"><rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<Views>  <rwc:tabview id="first" title="First" icon="Images/First.gif">
  </rwc:tabview>
</views>
</rwc:tabbedmultiview></td>
</tr>
</table></form>
  </body>
</html>
