<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerNavigationTabs.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.SecurityManagerNavigationTabs" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="securityManager" Src="SecurityManagerCurrentTenantControl.ascx" TagName="CurrentTenantControl" %>
<div style="text-align: right">
<securityManager:CurrentTenantControl id="SecurityManagerCurrentTenantControl" runat="server" EnableAbstractTenants="true"/>
</div>
<rubicon:TabbedMenu ID="TabbedMenu" runat="server">
  <Tabs>
    <rubicon:MainMenuTab ItemID="OrganizationalStructureTab" Text="$res:OrganizationalStructure">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <rubicon:SubMenuTab ItemID="UserTab" Text="$res:User">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="UserList" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="GroupTab" Text="$res:Group">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupList" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="TenantTab" Text="$res:Tenant">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="TenantList" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="PositionTab" Text="$res:Position">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="PositionList" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="GroupTypeTab" Text="$res:GroupType">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupTypeList" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
      </SubMenuTabs>
    </rubicon:MainMenuTab>
    <rubicon:MainMenuTab ItemID="AccessControlTab" Text="$res:AccessControl">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <rubicon:SubMenuTab ItemID="SecurableClassDefinitionTab" Text="$res:SecurableClassDefinition">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SecurableClassDefinitionList" WxeFunctionCommand-Parameters="&quot;Tenant|00000001-0000-0000-0000-000000000001|System.Guid&quot;" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
      </SubMenuTabs>
    </rubicon:MainMenuTab>
  </Tabs>
</rubicon:TabbedMenu>
