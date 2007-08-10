<%@ Page Language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
  <title>Test Tabbed Form</title>
  <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="Form" method="post" runat="server">
    <rubicon:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView">
      <TopControls>
        <rubicon:TabbedMenu ID="NavigationTabs" runat="server" StatusText="Status Text" SubMenuBackgroundColor-IsEmpty="True" SubMenuBackgroundColor-A="0" SubMenuBackgroundColor-B="0"
          SubMenuBackgroundColor-IsNamedColor="False" SubMenuBackgroundColor-IsKnownColor="False" SubMenuBackgroundColor-Name="0" SubMenuBackgroundColor-G="0"
          SubMenuBackgroundColor-R="0" SubMenuBackgroundColor-IsSystemColor="False">
          <Tabs>
            <rubicon:MainMenuTab Text="Tab 1" ItemID="Tab1">
              <SubMenuTabs>
                <rubicon:SubMenuTab Text="Event" ItemID="EventTab">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Href" ItemID="HrefTab">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Href" HrefCommand-Href="StartForm.aspx"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Client Wxe" ItemID="ClientWxeTab">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-MappingID="TestTabbedForm"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Invisible Tab" ItemID="InvisibleTab" IsVisible="False">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Disabled Tab" ItemID="DisabledTab" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
              </SubMenuTabs>
              <PersistedCommand>
                <rubicon:NavigationCommand Type="None"></rubicon:NavigationCommand>
              </PersistedCommand>
            </rubicon:MainMenuTab>
            <rubicon:MainMenuTab Text="Tab 2" ItemID="Tab2">
              <SubMenuTabs>
                <rubicon:SubMenuTab Text="Sub Tab 2.1" ItemID="SubTab1">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Sub Tab 2.2" ItemID="SubTab2">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
                <rubicon:SubMenuTab Text="Sub Tab 2.3" ItemID="SubTab23">
                  <PersistedCommand>
                    <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
                  </PersistedCommand>
                </rubicon:SubMenuTab>
              </SubMenuTabs>
              <PersistedCommand>
                <rubicon:NavigationCommand Type="None"></rubicon:NavigationCommand>
              </PersistedCommand>
            </rubicon:MainMenuTab>
            <rubicon:MainMenuTab Text="Tab 3" ItemID="Tab3" IsVisible="False">
              <PersistedCommand>
                <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
              </PersistedCommand>
            </rubicon:MainMenuTab>
            <rubicon:MainMenuTab Text="Tab 4" ItemID="Tab4">
              <PersistedCommand>
                <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
              </PersistedCommand>
            </rubicon:MainMenuTab>
            <rubicon:MainMenuTab Text="Tab 5" ItemID="Tab5" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
              <PersistedCommand>
                <rubicon:NavigationCommand Type="Event"></rubicon:NavigationCommand>
              </PersistedCommand>
            </rubicon:MainMenuTab>
          </Tabs>
        </rubicon:TabbedMenu>
        <system.web.ui.htmlcontrols.htmlgenericcontrol>Test Tabbed Form</system.web.ui.htmlcontrols.htmlgenericcontrol>
        <rubicon:ValidationStateViewer ID="ValidationStateViewer"></rubicon:ValidationStateViewer>
      </TopControls>
      <Views>
        <rubicon:TabView ID="first" Title="First">
          <rubicon:WebTabStrip ID="PagesTabStrip" runat="server" Style="margin: 3em">
          </rubicon:WebTabStrip>
          <asp:Literal ID="Literal" runat="server">
          01 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          02 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          03 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          04 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          05 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          06 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          07 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          08 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          09 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          10 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          11 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          12 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          13 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          14 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          15 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          16 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          17 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          18 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          19 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          20 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          21 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          22 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          23 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          24 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          25 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          26 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          27 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />          
          28 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />          
          29 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          30 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          31 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          32 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          33 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          34 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          35 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          36 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          37 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          38 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />          
          39 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          40 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          41 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          42 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          43 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          44 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          45 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          46 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          47 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />          
          48 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />          
          49 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          50 foo bar foo bar foo bar foo bar foo bar foo bar foo bar foo bar<br />
          </asp:Literal>
        </rubicon:TabView>
        <rubicon:TabView ID="second" Title="Second">
        </rubicon:TabView>
      </Views>
      <BottomControls>
        <rubicon:SmartHyperLink ID="SmartHyperLink1" runat="server" NavigateUrl="~/Start.aspx">test</rubicon:SmartHyperLink>
      </BottomControls>
    </rubicon:TabbedMultiView>
  </form>
</body>
</html>
