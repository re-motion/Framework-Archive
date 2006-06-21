<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditPositionControl.ascx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.EditPositionControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>

<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<rubicon:domainobjectdatasourcecontrol id="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.Position, Rubicon.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <rubicon:SmartLabel runat="server" id="PositionLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Name">
        <TextBoxStyle MaxLength="100" />
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="RolesField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" OnMenuItemClick="RolesField_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition ItemID="UserNameItem" PropertyPathIdentifier="User.DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition ItemID="GroupNameItem" PropertyPathIdentifier="Group.DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="ConcretePositionsField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ConcretePositions" OnMenuItemClick="ConcretePositionsField_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition ItemID="ConcretePositionsNameItem" PropertyPathIdentifier="Name">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition ItemID="GroupTypeNameItem" PropertyPathIdentifier="GroupType.Name">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <rubicon:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
          <rubicon:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <rubicon:BocMenuItemCommand />
            </PersistedCommand>
          </rubicon:BocMenuItem>
        </ListMenuItems>
      </rubicon:BocList>
    </td>
  </tr>
</table>
