<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupTypePositionControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupTypePositionControl" %>

<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Rubicon.SecurityManager.Domain.OrganizationalStructure.GroupTypePosition, Rubicon.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <rubicon:SmartLabel runat="server" id="GroupTypePositionLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocReferenceValue runat="server" ID="GroupTypeField" DataSourceControl="CurrentObject" PropertyIdentifier="GroupType">
        <PersistedCommand>
          <rubicon:BocCommand />
        </PersistedCommand>
      </rubicon:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <rubicon:BocReferenceValue runat="server" ID="PositionField" DataSourceControl="CurrentObject" PropertyIdentifier="Position">
        <PersistedCommand>
          <rubicon:BocCommand />
        </PersistedCommand>
      </rubicon:BocReferenceValue>
    </td>
  </tr>
</table>
