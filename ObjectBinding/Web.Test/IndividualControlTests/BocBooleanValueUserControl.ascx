<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocBooleanValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocBooleanValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obw:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id="DeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" nullitemerrormessage="Eingabe erforderlich" falsedescription="nein" nulldescription="undefiniert" truedescription="ja" ></obw:bocbooleanvalue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id="DeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id="ReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" readonly="True"></obw:bocbooleanvalue></td>
    <td>bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id=UnboundDeceasedField runat="server" Width="150px" required="False" showdescription="False"></obw:bocbooleanvalue></td>
    <td>unbound, value not set, required= false, description=false</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id=UnboundReadOnlyDeceasedField runat="server" Width="150px" ReadOnly="True" height="8px"></obw:bocbooleanvalue></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id="DisabledDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" nullitemerrormessage="Eingabe erforderlich" falsedescription="nein" nulldescription="undefiniert" truedescription="ja" enabled=false></obw:bocbooleanvalue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id="DisabledReadOnlyDeceasedField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="300px" readonly="True" enabled=false></obw:bocbooleanvalue></td>
    <td>disabled, bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id=DisabledUnboundDeceasedField runat="server" Width="150px" required="False" enabled=false></obw:bocbooleanvalue></td>
    <td> disabled, unbound, value set, required= false</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id=DisabledUnboundReadOnlyDeceasedField runat="server" Width="150px" ReadOnly="True" height="8px" enabled=false></obw:bocbooleanvalue></td>
    <td>disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p>Deceased Field Checked Changed: <asp:label id="DeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><rubicon:webbutton id="DeceasedTestSetNullButton" runat="server" Text="Deceased Set Null" width="220px"/><rubicon:webbutton id="DeceasedTestToggleValueButton" runat="server" Text="Deceased Toggle Value" width="220px"/></p>
<p><rubicon:webbutton id="ReadOnlyDeceasedTestSetNullButton" runat="server" Text="Read Only Deceased Set Null" width="220px"/><rubicon:webbutton id="ReadOnlyDeceasedTestToggleValueButton" runat="server" Text="Read Only Deceased Toggle Value" width="220px"/></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><obr:reflectionbusinessobjectdatasourcecontrol id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/></div>
