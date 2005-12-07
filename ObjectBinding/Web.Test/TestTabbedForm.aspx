<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm"%>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
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
      <rwc:TabbedMenu id="NavigationTabs" runat="server" width="100%">
<tabs>
<rwc:MainMenuTab ItemID="Tab1" Text="Tab 1">
<submenutabs>
<rwc:SubMenuTab ItemID="Tab1" Text="Event">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab ItemID="Tab2" Text="Href">
<persistedcommand>
<rwc:Command Type="Href" HrefCommand-Href="StartForm.aspx"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab ItemID="Tab3" Text="Wxe">
<persistedcommand>
<rwc:Command Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-TypeName="OBWTest.TestTabbedFormWxeFunction,OBWTest"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab ItemID="Tab2" Text="Tab 2">
<submenutabs>
<rwc:SubMenuTab ItemID="SubTab1" Text="Sub Tab 2.1">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab ItemID="SubTab2" Text="Sub Tab 2.2">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab ItemID="SubTab23" Text="Sub Tab 2.3">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab ItemID="Tab3" Text="Tab 3">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
</Tabs>
</rwc:TabbedMenu>
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
