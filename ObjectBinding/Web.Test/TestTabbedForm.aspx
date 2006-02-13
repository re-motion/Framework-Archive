<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm"%>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
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
<form id=Form method=post runat="server"><rwc:tabbedmenu id=NavigationTabs runat="server" StatusText="Status Text" SubMenuBackgroundColor-IsEmpty="True" SubMenuBackgroundColor-A="0" SubMenuBackgroundColor-B="0" SubMenuBackgroundColor-IsNamedColor="False" SubMenuBackgroundColor-IsKnownColor="False" SubMenuBackgroundColor-Name="0" SubMenuBackgroundColor-G="0" SubMenuBackgroundColor-R="0" SubMenuBackgroundColor-IsSystemColor="False">
<tabs>
<rwc:MainMenuTab Text="Tab 1" ItemID="Tab1">
<submenutabs>
<rwc:SubMenuTab Text="Event" ItemID="EventTab">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Href" ItemID="HrefTab">
<persistedcommand>
<rwc:NavigationCommand Type="Href" HrefCommand-Href="StartForm.aspx"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Client Wxe" ItemID="ClientWxeTab">
<persistedcommand>
<rwc:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-MappingID="TestTabbedForm"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Invisible Tab" ItemID="InvisibleTab" IsVisible="False">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Disabled Tab" ItemID="DisabledTab" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:NavigationCommand Type="None"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 2" ItemID="Tab2">
<submenutabs>
<rwc:SubMenuTab Text="Sub Tab 2.1" ItemID="SubTab1">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Sub Tab 2.2" ItemID="SubTab2">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
<rwc:SubMenuTab Text="Sub Tab 2.3" ItemID="SubTab23">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rwc:NavigationCommand Type="None"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 3" ItemID="Tab3" IsVisible="False">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 4" ItemID="Tab4">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:MainMenuTab>
<rwc:MainMenuTab Text="Tab 5" ItemID="Tab5" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
<persistedcommand>
<rwc:NavigationCommand Type="Event"></rwc:NavigationCommand>
</PersistedCommand>
</rwc:MainMenuTab>
</Tabs>
</rwc:tabbedmenu><rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<topcontrols>
<system.web.ui.htmlcontrols.htmlgenericcontrol>Test Tabbed Form</System.Web.UI.HtmlControls.HtmlGenericControl>
<rwc:ValidationStateViewer ID="ValidationStateViewer"></rwc:ValidationStateViewer>
</TopControls>
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
