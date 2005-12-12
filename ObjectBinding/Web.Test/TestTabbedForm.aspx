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
      <rwc:TabbedMenu id="NavigationTabs" runat="server" width="100%">
<tabs>
<rwc:MainMenuTab Text="Tab 1" ItemID="Tab1">
<submenutabs>
<rwc:SubMenuTab Text="Event" ItemID="Tab1">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Href" ItemID="Tab2">
<persistedcommand>
<rwc:Command Type="Href" HrefCommand-Href="StartForm.aspx"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Wxe" ItemID="Tab3">
<persistedcommand>
<rwc:Command Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-TypeName="OBWTest.TestTabbedFormWxeFunction,OBWTest"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 2" ItemID="Tab2">
<submenutabs>
<rwc:SubMenuTab Text="Sub Tab 2.1" ItemID="SubTab1">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Sub Tab 2.2" ItemID="SubTab2">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Sub Tab 2.3" ItemID="SubTab23">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 3" ItemID="Tab3" IsVisible="False">
<persistedcommand>
<rwc:Command Type="Event"></rwc:Command>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 4" ItemID="Tab4">
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
