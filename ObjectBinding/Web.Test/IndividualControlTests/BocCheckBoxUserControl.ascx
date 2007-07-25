<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocCheckBoxUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocCheckBoxUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>

<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obw:boctextvalue id=FirstNameField runat="server" datasourcecontrol="CurrentObject" ReadOnly="True" PropertyIdentifier="FirstName"></obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" datasourcecontrol="CurrentObject" ReadOnly="True" PropertyIdentifier="LastName"></obw:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=DeceasedField runat="server" datasourcecontrol="CurrentObject" truedescription="ja" nulldescription="undefiniert" falsedescription="nein" nullitemerrormessage="Eingabe erforderlich" width="300px" propertyidentifier="Deceased" showdescription="True"></obw:boccheckbox></td>
    <td>bound, description=true</td>
    <td style="WIDTH: 20%"><asp:label id=DeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=ReadOnlyDeceasedField runat="server" datasourcecontrol="CurrentObject" width="300px" propertyidentifier="Deceased" readonly="True" showdescription="True"></obw:boccheckbox></td>
    <td>bound, read only, description= true</td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=UnboundDeceasedField runat="server" Width="150px"></obw:boccheckbox></td>
    <td>unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=UnboundReadOnlyDeceasedField runat="server" ReadOnly="True" Width="150px" height="8px"></obw:boccheckbox></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=DisabledDeceasedField runat="server" datasourcecontrol="CurrentObject" truedescription="ja" nulldescription="undefiniert" falsedescription="nein" nullitemerrormessage="Eingabe erforderlich" width="300px" propertyidentifier="Deceased" showdescription="True" enabled="false"></obw:boccheckbox></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=DisabledReadOnlyDeceasedField runat="server" datasourcecontrol="CurrentObject" width="300px" propertyidentifier="Deceased" readonly="True" showdescription="True" enabled="false"></obw:boccheckbox></td>
    <td>disabled, bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=DisabledUnboundDeceasedField runat="server" Width="150px" enabled="false"></obw:boccheckbox></td>
    <td>disabled, unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:boccheckbox id=DisabledUnboundReadOnlyDeceasedField runat="server" ReadOnly="True" Width="150px" height="8px" enabled="false"></obw:boccheckbox></td>
    <td>disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyDeceasedFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr></table>
<p>Deceased Field Checked Changed: <asp:label id=DeceasedFieldCheckedChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><rubicon:webbutton id=DeceasedTestSetNullButton runat="server" width="220px" Text="Deceased Set Null"/><rubicon:webbutton id=DeceasedTestToggleValueButton runat="server" width="220px" Text="Deceased Toggle Value"/></p>
<p><rubicon:webbutton id=ReadOnlyDeceasedTestSetNullButton runat="server" width="220px" Text="Read Only Deceased Set Null"/><rubicon:webbutton id=ReadOnlyDeceasedTestToggleValueButton runat="server" width="220px" Text="Read Only Deceased Toggle Value"/></p>
