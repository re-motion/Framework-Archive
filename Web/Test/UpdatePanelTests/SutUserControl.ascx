<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SutUserControl.ascx.cs"
  Inherits="Rubicon.Web.Test.UpdatePanelTests.SutUserControl" %>
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
        <asp:Button ID="AsyncPostBackInsideUpdatePanelButton" runat="server" Text="Async PostBack Inside Update Panel" /><br />
        <asp:Button ID="SyncPostBackInsideUpdatePanelButton" runat="server" Text="Sync PostBack Inside Update Panel" /><br />
        <asp:LinkButton ID="AsyncPostBackInsideUpdatePanelLinkButton" runat="server" Text="Async PostBack Inside Update Panel" /><br />
        <asp:LinkButton ID="SyncPostBackInsideUpdatePanelLinkButton" runat="server" Text="Sync PostBack Inside Update Panel" /><br />
        <asp:HyperLink ID="SyncCommandInsideUpdatePanelHyperLink" runat="server">Sync Command Inside Update Panel HyperLink</asp:HyperLink><br />
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
  <asp:Button ID="AsyncPostBackOutsideUpdatePanelButton" runat="server" Text="Async PostBack Outside Update Panel" /><br />
  <asp:Button ID="SyncPostBackOutsideUpdatePanelButton" runat="server" Text="Sync PostBack Outside Update Panel" /><br />
  <asp:LinkButton ID="AsyncPostBackOutsideUpdatePanelLinkButton" runat="server" Text="Async PostBack Outside Update Panel" /><br />
  <asp:LinkButton ID="SyncPostBackOutsideUpdatePanelLinkButton" runat="server" Text="Sync PostBack Outside Update Panel" /><br />
</div>
