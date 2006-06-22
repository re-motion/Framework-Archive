<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupTypeListControl.ascx.cs" Inherits="Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI.GroupTypeListControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.OrganizationalStructure.GroupType, Rubicon.SecurityManager" />
<rubicon:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <rubicon:SmartLabel runat="server" id="GroupTypeListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <rubicon:BocList ID="GroupTypeList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="GroupTypeList_ListItemCommandClick">
              <FixedColumns>
                <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Name">
                  <PersistedCommand>
                    <rubicon:BocListItemCommand Type="Event" />
                  </PersistedCommand>
                </rubicon:BocSimpleColumnDefinition>
              </FixedColumns>
            </rubicon:BocList>
          </td>
        </tr>
      </table>
    </td>
  </tr>
  <tr>
    <td style="padding-top: 5px;">
      <rubicon:WebButton ID="NewGroupTypeButton" runat="server" OnClick="NewGroupTypeButton_Click" />
    </td>
  </tr>
</table>
