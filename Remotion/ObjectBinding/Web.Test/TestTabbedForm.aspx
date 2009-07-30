<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm" Title="Test TabbedMultiView" %>
<%@ Import Namespace="OBWTest"%>

<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <remotion:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView">
      <TopControls>
        <remotion:TabbedMenu ID="NavigationTabs" runat="server" StatusText="Status Text" SubMenuBackgroundColor-IsEmpty="True" SubMenuBackgroundColor-A="0" SubMenuBackgroundColor-B="0"
          SubMenuBackgroundColor-IsNamedColor="False" SubMenuBackgroundColor-IsKnownColor="False" SubMenuBackgroundColor-Name="0" SubMenuBackgroundColor-G="0"
          SubMenuBackgroundColor-R="0" SubMenuBackgroundColor-IsSystemColor="False">
          <Tabs>
            <remotion:MainMenuTab Text="Tab 1" ItemID="Tab1">
              <SubMenuTabs>
                <remotion:SubMenuTab Text="Event" ItemID="EventTab">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Href" ItemID="HrefTab">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Href" HrefCommand-Href="StartForm.aspx"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Client Wxe" ItemID="ClientWxeTab">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="false" WxeFunctionCommand-MappingID="TestTabbedForm"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Invisible Tab" ItemID="InvisibleTab" IsVisible="False">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Disabled Tab" ItemID="DisabledTab" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
              </SubMenuTabs>
              <PersistedCommand>
                <remotion:NavigationCommand Type="None"></remotion:NavigationCommand>
              </PersistedCommand>
            </remotion:MainMenuTab>
            <remotion:MainMenuTab Text="Tab 2" ItemID="Tab2">
              <SubMenuTabs>
                <remotion:SubMenuTab Text="Sub Tab 2.1" ItemID="SubTab1">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Sub Tab 2.2" ItemID="SubTab2">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
                <remotion:SubMenuTab Text="Sub Tab 2.3" ItemID="SubTab23">
                  <PersistedCommand>
                    <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
                  </PersistedCommand>
                </remotion:SubMenuTab>
              </SubMenuTabs>
              <PersistedCommand>
                <remotion:NavigationCommand Type="None"></remotion:NavigationCommand>
              </PersistedCommand>
            </remotion:MainMenuTab>
            <remotion:MainMenuTab Text="Tab 3" ItemID="Tab3" IsVisible="False">
              <PersistedCommand>
                <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
              </PersistedCommand>
            </remotion:MainMenuTab>
            <remotion:MainMenuTab Text="Tab 4" ItemID="Tab4">
              <PersistedCommand>
                <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
              </PersistedCommand>
            </remotion:MainMenuTab>
            <remotion:MainMenuTab Text="Tab 5" ItemID="Tab5" Icon-Url="Images/DeleteItem.gif" IsDisabled="True">
              <PersistedCommand>
                <remotion:NavigationCommand Type="Event"></remotion:NavigationCommand>
              </PersistedCommand>
            </remotion:MainMenuTab>
          </Tabs>
        </remotion:TabbedMenu>
        <system.web.ui.htmlcontrols.htmlgenericcontrol>Test Tabbed Form</system.web.ui.htmlcontrols.htmlgenericcontrol>
        <remotion:ValidationStateViewer ID="ValidationStateViewer"></remotion:ValidationStateViewer>
      </TopControls>
      <Views>
        <remotion:TabView ID="first" Title="First">
          <remotion:WebTabStrip ID="PagesTabStrip" runat="server" Style="margin: 3em">
          </remotion:WebTabStrip>
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
        </remotion:TabView>
        <remotion:TabView ID="second" Title="Second">
        </remotion:TabView>
      </Views>
      <BottomControls>
        <remotion:SmartHyperLink ID="SmartHyperLink1" runat="server" NavigateUrl="~/Start.aspx">test</remotion:SmartHyperLink>
      </BottomControls>
    </remotion:TabbedMultiView>
</asp:Content>
