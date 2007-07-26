<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm"%>



<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head><title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rubicon:tabbedmenu id=NavigationTabs runat="server" StatusText="Status Text" SubMenuBackgroundColor-IsEmpty="True" SubMenuBackgroundColor-A="0" SubMenuBackgroundColor-B="0" SubMenuBackgroundColor-IsNamedColor="False" SubMenuBackgroundColor-IsKnownColor="False" SubMenuBackgroundColor-Name="0" SubMenuBackgroundColor-G="0" SubMenuBackgroundColor-R="0" SubMenuBackgroundColor-IsSystemColor="False">
<tabs>
<rubicon:MainMenuTab Text="Tab 1" ItemID="Tab1">
<submenutabs>
<rubicon:SubMenuTab Text="Event" ItemID="EventTab">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Href" ItemID="HrefTab">
<persistedcommand>
<rubicon:NavigationCommand Type="Href" HrefCommand-Href="StartForm.aspx"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Client Wxe" ItemID="ClientWxeTab">
<persistedcommand>
<rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-MappingID="TestTabbedForm"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Invisible Tab" ItemID="InvisibleTab" IsVisible="False">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Disabled Tab" ItemID="DisabledTab" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rubicon:NavigationCommand Type="None"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
<rubicon:MainMenuTab Text="Tab 2" ItemID="Tab2">
<submenutabs>
<rubicon:SubMenuTab Text="Sub Tab 2.1" ItemID="SubTab1">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Sub Tab 2.2" ItemID="SubTab2">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
<rubicon:SubMenuTab Text="Sub Tab 2.3" ItemID="SubTab23">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>
</SubMenuTabs>

<persistedcommand>
<rubicon:NavigationCommand Type="None"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
<rubicon:MainMenuTab Text="Tab 3" ItemID="Tab3" IsVisible="False">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
<rubicon:MainMenuTab Text="Tab 4" ItemID="Tab4">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
<rubicon:MainMenuTab Text="Tab 5" ItemID="Tab5" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
<persistedcommand>
<rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
</Tabs>
</rubicon:tabbedmenu><rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<topcontrols>
<system.web.ui.htmlcontrols.htmlgenericcontrol>Test Tabbed Form</System.Web.UI.HtmlControls.HtmlGenericControl>
<rubicon:ValidationStateViewer ID="ValidationStateViewer"></rubicon:ValidationStateViewer>
</TopControls>
<views> 
 <rubicon:tabview id="first" title="First">
      <rubicon:webtabstrip id="PagesTabStrip" runat="server" style="margin:3em"></rubicon:webtabstrip>
 </rubicon:tabview>
 <rubicon:tabview id="second" title="Second">
 </rubicon:tabview>
</Views>
</rubicon:tabbedmultiview><rubicon:smarthyperlink id=SmartHyperLink1 runat="server" NavigateUrl="~/Start.aspx">test</rubicon:smarthyperlink>&nbsp;</form>
  </body>
</html>
