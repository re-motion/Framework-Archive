



<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocBooleanValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocBooleanValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="DeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" nullitemerrormessage="Eingabe erforderlich" falsedescription="nein" nulldescription="undefiniert" truedescription="ja" ></rubicon:bocbooleanvalue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id="DeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="ReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" readonly="True"></rubicon:bocbooleanvalue></td>
    <td>bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id=UnboundDeceasedField runat="server" Width="150px" required="False" showdescription="False"></rubicon:bocbooleanvalue></td>
    <td>unbound, value not set, required= false, description=false</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id=UnboundReadOnlyDeceasedField runat="server" Width="150px" ReadOnly="True" height="8px"></rubicon:bocbooleanvalue></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="DisabledDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" nullitemerrormessage="Eingabe erforderlich" falsedescription="nein" nulldescription="undefiniert" truedescription="ja" enabled=false></rubicon:bocbooleanvalue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="DisabledReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" readonly="True" enabled=false></rubicon:bocbooleanvalue></td>
    <td>disabled, bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id=DisabledUnboundDeceasedField runat="server" Width="150px" required="False" enabled=false></rubicon:bocbooleanvalue></td>
    <td> disabled, unbound, value set, required= false</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id=DisabledUnboundReadOnlyDeceasedField runat="server" Width="150px" ReadOnly="True" height="8px" enabled=false></rubicon:bocbooleanvalue></td>
    <td>disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p>Deceased Field Checked Changed: <asp:label id="DeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><rubicon:webbutton id="DeceasedTestSetNullButton" runat="server" Text="Deceased Set Null" width="220px"/><rubicon:webbutton id="DeceasedTestToggleValueButton" runat="server" Text="Deceased Toggle Value" width="220px"/></p>
<p><rubicon:webbutton id="ReadOnlyDeceasedTestSetNullButton" runat="server" Text="Read Only Deceased Set Null" width="220px"/><rubicon:webbutton id="ReadOnlyDeceasedTestToggleValueButton" runat="server" Text="Read Only Deceased Toggle Value" width="220px"/></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/></div>
