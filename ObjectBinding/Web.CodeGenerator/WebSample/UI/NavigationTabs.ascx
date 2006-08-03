<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NavigationTabs.ascx.cs" Inherits="WebSample.UI.NavigationTabs" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>

<rubicon:TabbedMenu ID="TheTabbedMenu" runat="server">
  <Tabs>
    
    <rubicon:MainMenuTab ItemID="PersonTab" Text="Person">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <rubicon:SubMenuTab ItemID="EditPersonTab" Text="$res:New">
          <PersistedCommand>
            <rubicon:NavigationCommand HrefCommand-Href="EditPerson.wxe" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="SearchPersonTab" Text="$res:List">
          <PersistedCommand>
            <rubicon:NavigationCommand HrefCommand-Href="SearchPerson.wxe" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
      </SubMenuTabs>
    </rubicon:MainMenuTab>

  </Tabs>
</rubicon:TabbedMenu>
