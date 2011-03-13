<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SutUserControl.ascx.cs"
  Inherits="Remotion.Web.Test.UpdatePanelTests.SutUserControl" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server" />
<asp:UpdatePanel ID="UpdatePanel" runat="server">
  <ContentTemplate>
    <div style="border: solid 1px black; padding: 1em;">
      <b>Update Panel</b>
      <div>
        Async PostBacks:
        <asp:Label ID="PostBackCountInsideUpdatePanelLabel" runat="server" Text="###" EnableViewState="false" />
        <br />
        Last PostBack:
        <asp:Label ID="LastPostBackInsideUpdatePanelLabel" runat="server" Text="###" EnableViewState="false" />
      </div>
      <div>
        <asp:Button ID="AsyncPostBackInsideUpdatePanelButton" runat="server" Text="Button: Async PostBack Inside Update Panel" /><br />
        <asp:Button ID="SyncPostBackInsideUpdatePanelButton" runat="server" Text="Button: Sync PostBack Inside Update Panel" /><br />
        <asp:LinkButton ID="AsyncPostBackInsideUpdatePanelLinkButton" runat="server" Text="LinkButton: Async PostBack Inside Update Panel" /><br />
        <asp:LinkButton ID="SyncPostBackInsideUpdatePanelLinkButton" runat="server" Text="LinkButton: Sync PostBack Inside Update Panel" /><br />
        <asp:HyperLink ID="AsyncCommandInsideUpdatePanelHyperLink" runat="server">HyperLink: Async Command Inside Update Panel</asp:HyperLink><br />
        <asp:HyperLink ID="SyncCommandInsideUpdatePanelHyperLink" runat="server">HyperLink: Sync Command Inside Update Panel</asp:HyperLink><br />
        <remotion:WebButton ID="AsyncPostBackInsideUpdatePanelWebButton" runat="server" Text="WebButton: Async PostBack Inside Update Panel" /><br />
        <remotion:WebButton ID="SyncPostBackInsideUpdatePanelWebButton" runat="server" Text="WebButton: Sync PostBack Inside Update Panel" RequiresSynchronousPostBack="true" /><br />
        <br />
        <remotion:DropDownMenu ID="DropDownMenuInsideUpdatePanel" runat="server" TitleText="DropDownMenu Inside UpdatePanel" Width="20em">
        <MenuItems>
          <remotion:WebMenuItem ItemID="EventWithSyncPostBack" Text="Event With Sync PostBack">
            <PersistedCommand>
              <remotion:Command Type="Event" EventCommand-RequiresSynchronousPostBack="True" />
            </PersistedCommand>
          </remotion:WebMenuItem>
          <remotion:WebMenuItem ItemID="EventWithAsyncPostBack" Text="Event With Async PostBack">
            <PersistedCommand>
              <remotion:Command Type="Event" />
            </PersistedCommand>
          </remotion:WebMenuItem>
        </MenuItems>
        </remotion:DropDownMenu>
      </div>
    </div>
  </ContentTemplate>
  <Triggers>
    <asp:AsyncPostBackTrigger ControlID="AsyncPostBackOutsideUpdatePanelButton" />
    <asp:AsyncPostBackTrigger ControlID="AsyncPostBackOutsideUpdatePanelLinkButton" />
    <asp:PostBackTrigger ControlID="SyncPostBackInsideUpdatePanelButton" />
    <asp:PostBackTrigger ControlID="SyncPostBackInsideUpdatePanelLinkButton" />
  </Triggers>
</asp:UpdatePanel>
<div>
  Sync PostBacks:
  <asp:Label ID="PostBackCountOutsideUpdatePanelLabel" runat="server" Text="###" EnableViewState="false" />
  <br />
  Last PostBack:
  <asp:Label ID="LastPostBackOutsideUpdatePanelLabel" runat="server" Text="###" EnableViewState="false" />
</div>
<div>
  <asp:Button ID="AsyncPostBackOutsideUpdatePanelButton" runat="server" Text="Button: Async PostBack Outside Update Panel" /><br />
  <asp:Button ID="SyncPostBackOutsideUpdatePanelButton" runat="server" Text="Button: Sync PostBack Outside Update Panel" /><br />
  <asp:LinkButton ID="AsyncPostBackOutsideUpdatePanelLinkButton" runat="server" Text="LinkButton: Async PostBack Outside Update Panel" /><br />
  <asp:LinkButton ID="SyncPostBackOutsideUpdatePanelLinkButton" runat="server" Text="LinkButton: Sync PostBack Outside Update Panel" /><br />
</div>
