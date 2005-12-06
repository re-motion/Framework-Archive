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
      <rwc:TabStripMenu id="NavigationTabs" runat="server" width="100%">
<tabs>
<rwc:TabStripMainMenuItem ItemID="Tab1" Text="Tab 1">
<submenutabs>
<rwc:TabStripSubMenuItem ItemID="Tab1" Text="Event">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
<rwc:TabStripSubMenuItem ItemID="Tab2" Text="Href">
<persistedcommand>
<rwc:Command Type="Href" HrefCommand-Href="StartForm.aspx"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
<rwc:TabStripSubMenuItem ItemID="Tab3" Text="Wxe">
<persistedcommand>
<rwc:Command Type="WxeFunction" WxeFunctionCommand-TypeName="OBWTest.TestTabbedFormWxeFunction,OBWTest"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripMainMenuItem>
<rwc:TabStripMainMenuItem ItemID="Tab2" Text="Tab 2">
<submenutabs>
<rwc:TabStripSubMenuItem ItemID="SubTab1" Text="Sub Tab 2.1">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
<rwc:TabStripSubMenuItem ItemID="SubTab2" Text="Sub Tab 2.2">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
<rwc:TabStripSubMenuItem ItemID="SubTab23" Text="Sub Tab 2.3">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripSubMenuItem>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripMainMenuItem>
<rwc:TabStripMainMenuItem ItemID="Tab3" Text="Tab 3">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:TabStripMainMenuItem>
</Tabs>
</rwc:TabStripMenu>
<rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<topcontrols>
      <h1>Test Tabbed Form</h1>
      <rwc:validationstateviewer id="ValidationStateViewer" runat="server" visible="true"></rwc:validationstateviewer>
</topcontrols>
<views> 
 <rwc:tabview id="first" title="First">
      <rwc:webtabstrip id="PagesTabStrip" runat="server" style="margin:3em"></rwc:webtabstrip>
 </rwc:tabview>
 <rwc:tabview id="second" title="Second">
 </rwc:tabview>
</Views>
</rwc:tabbedmultiview></form>
  </body>
</html>
