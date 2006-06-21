<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUserControl.ascx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.EditUserControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>

<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<rubicon:domainobjectdatasourcecontrol id="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.User, Rubicon.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <rubicon:SmartLabel runat="server" id="UserLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue runat="server" ID="UserNameField" DataSourceControl="CurrentObject" PropertyIdentifier="UserName"></rubicon:BocTextValue>    
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue ID="TitleField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Title">
        <TextBoxStyle MaxLength="100" />
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue ID="FirstnameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="FirstName">
        <TextBoxStyle MaxLength="100" />
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocTextValue ID="LastNameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName">
        <TextBoxStyle MaxLength="100" />
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocReferenceValue ID="GroupField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Group">
        <PersistedCommand>
          <rubicon:BocCommand />
        </PersistedCommand>
      </rubicon:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocList ID="RolesField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" OnMenuItemClick="RolesField_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition ItemID="GroupNameItem" PropertyPathIdentifier="Group.DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand />
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition ItemID="PositionNameItem" PropertyPathIdentifier="Position.Name">
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
