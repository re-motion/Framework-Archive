<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecurityManagerNavigationTabs.ascx.cs" Inherits="Rubicon.SecurityManager.Client.Web.UI.SecurityManagerNavigationTabs" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
  
<div id="UserNameLabel" runat="server" style="text-align:right">
###
</div>
          
<rubicon:TabbedMenu ID="TabbedMenu" runat="server" >
  <Tabs>
    <rubicon:MainMenuTab ItemID="OrganisationalStructureTab" Text="$res:OrganisationalStructure">
      <PersistedCommand>
      <rubicon:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <rubicon:SubMenuTab ItemID="UserTab" Text="$res:User">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="UserList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
          </PersistedCommand>
        </rubicon:SubMenuTab>  
        <rubicon:SubMenuTab ItemID="GroupTab" Text="$res:Group">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
          </PersistedCommand>
        </rubicon:SubMenuTab>  
        <rubicon:SubMenuTab ItemID="PositionTab" Text="$res:Position">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="PositionList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
          </PersistedCommand>
        </rubicon:SubMenuTab>  
        <rubicon:SubMenuTab ItemID="GroupTypeTab" Text="$res:GroupType">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupTypeList" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
          </PersistedCommand>
        </rubicon:SubMenuTab>  
      </SubMenuTabs>
    </rubicon:MainMenuTab>
  </Tabs>
</rubicon:TabbedMenu>
 
