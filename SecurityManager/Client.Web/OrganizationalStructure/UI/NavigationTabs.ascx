<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationTabs.ascx.cs" Inherits="Rubicon.Kis.Client.Web.UI.AdministrationUI.NavigationTabs" %>
<%@ Register TagPrefix="Rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
  
<div id="UserNameLabel" runat="server" style="text-align:right">
###
</div>
          
<Rubicon:TabbedMenu ID="TabbedMenu" runat="server" >
  <Tabs>
    <rubicon:MainMenuTab ItemID="UserTab" Text="$res:User">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="UserList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="GroupTab" Text="$res:Group">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="PositionTab" Text="$res:Position">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="PositionList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="GroupTypeTab" Text="$res:GroupType">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupTypeList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
  </Tabs>
</rubicon:TabbedMenu>
 
