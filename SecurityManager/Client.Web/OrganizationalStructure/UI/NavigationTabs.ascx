<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationTabs.ascx.cs" Inherits="Rubicon.Kis.Client.Web.UI.AdministrationUI.NavigationTabs" %>
<%@ Register TagPrefix="Rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
  
<table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
  <tr>
    <td style="width: 50%" align="right">
      <obw:BocTextValue ID="UserFullNameTextValue" CssClass="headerAreaTitleLabel" runat="server" ReadOnly="True">
        <TextBoxStyle />
      </obw:BocTextValue>
    </td>
  </tr>
</table>
          
<Rubicon:TabbedMenu ID="TabbedMenu" runat="server" >
  <Tabs>
    <rubicon:MainMenuTab ItemID="UserTab" Text="$res:User">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SearchUser" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="GroupTab" Text="$res:Group">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SearchGroup" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="PositionTab" Text="$res:Position">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SearchPosition" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
    <rubicon:MainMenuTab ItemID="GroupTypeTab" Text="$res:GroupType">
      <PersistedCommand>
        <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SearchGroupType" WxeFunctionCommand-Parameters="&quot;Client|00000001-0000-0000-0000-000000000001|System.Guid&quot;"  />
      </PersistedCommand>
    </rubicon:MainMenuTab>  
  </Tabs>
</rubicon:TabbedMenu>
 
