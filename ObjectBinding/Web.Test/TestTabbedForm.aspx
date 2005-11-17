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
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<TopControls>
      <h1>Test Tabbed Form</H1>
      <rwc:validationstateviewer id=ValidationStateViewer runat="server" visible="true"></rwc:validationstateviewer>
      <rwc:webtabstrip id=PagesTabStrip runat="server" width="100%"></rwc:webtabstrip>
</topcontrols>
<views> 
 <rwc:tabview id="first" title="First">
 </rwc:tabview>
 <rwc:tabview id="second" title="Second">
 </rwc:tabview>
</Views>
</rwc:tabbedmultiview></FORM>
  </body>
</html>
