<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocDateTimeValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocDateTimeValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><obr:reflectionbusinessobjectdatasourcecontrol id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/></div>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=4><obw:boctextvalue id=FirstNameField runat="server" ReadOnly="True" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject"></obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" ReadOnly="True" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject"></obw:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=BirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject"  invalidtimeerrormessage="Ungültige Zeit" invaliddateerrormessage="Ungültiges Datum" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" incompleteerrormessage="Unvollständige Daten" width="300px" showseconds="False"></obw:bocdatetimevalue></td>
          <td>
            <p>bound</p></td>
          <td style="WIDTH: 20%"><asp:label id=BirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=ReadOnlyBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" readonly="True" showseconds="False" width="300px"></obw:bocdatetimevalue></td>
          <td>
            <p>bound, read-only</p></td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=UnboundBirthdayField runat="server" width="300px" readonly="False" required="False"></obw:bocdatetimevalue></td>
          <td>
            <p>unbound, value not set, not required</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id="UnboundRequiredBirthdayField" runat="server" width="300px" required="True"></obw:bocdatetimevalue></td>
          <td>
            <p>unbound, value not set</p></td>
          <td style="WIDTH: 20%"><asp:label id="UnboundRequiredBirthdayFieldLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=UnboundReadOnlyBirthdayField runat="server" readonly="True"></obw:bocdatetimevalue></td>
          <td>
            <p>unbound, value set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=DateOfDeathField runat="server" PropertyIdentifier="DateOfDeath" datasourcecontrol="CurrentObject" width="300px" readonly="False"></obw:bocdatetimevalue></td>
          <td>
            <p>bound</p></td>
          <td style="WIDTH: 20%"><asp:label id=DateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=ReadOnlyDateOfDeathField runat="server" PropertyIdentifier="DateOfDeath" datasourcecontrol="CurrentObject" readonly="True"></obw:bocdatetimevalue></td>
          <td>
            <p>bound, read-only</p></td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=UnboundDateOfDeathField runat="server" readonly="False" required="False"></obw:bocdatetimevalue></td>
          <td>
            <p>unbound, value not set, not required</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=UnboundReadOnlyDateOfDeathField runat="server" readonly="True"></obw:bocdatetimevalue></td>
          <td>
            <p>unbound, value set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyDateOfDeathFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td>Today</td>
          <td><obw:bocdatetimevalue id=DirectlySetBocDateTimeValueField runat="server" readonly="False" required="False" valuetype="Date"></obw:bocdatetimevalue></td>
          <td>
            <p>directly set, not required</p></td>
          <td style="WIDTH: 20%"><asp:label id=DirectlySetBocDateTimeValueFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td>Today</td>
          <td><obw:bocdatetimevalue id=ReadOnlyDirectlySetBocDateTimeValueField runat="server" readonly="True" valuetype="Date"></obw:bocdatetimevalue></td>
          <td>
            <p>directly set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=DisabledBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject"  invalidtimeerrormessage="Ungültige Zeit" invaliddateerrormessage="Ungültiges Datum" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" incompleteerrormessage="Unvollständige Daten" width="300px" showseconds="False" enabled=false></obw:bocdatetimevalue></td>
          <td>
            <p>disabled, bound</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=DisabledReadOnlyBirthdayField runat="server" PropertyIdentifier="DateOfBirth" datasourcecontrol="CurrentObject" readonly="True" showseconds="False" enabled=false></obw:bocdatetimevalue></td>
          <td>
            <p>disabled, bound, read-only</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=DisabledUnboundBirthdayField runat="server" width="300px" readonly="False" required="False" enabled=false></obw:bocdatetimevalue></td>
          <td>
            <p> disabled, unbound, value set, not required</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><obw:bocdatetimevalue id=DisabledUnboundReadOnlyBirthdayField runat="server" readonly="True" enabled=false></obw:bocdatetimevalue></td>
          <td>
            <p>disabled, unbound, value set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyBirthdayFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr></table>
      <p><rubicon:webbutton id=BirthdayTestSetNullButton runat="server" width="220px" Text="Birthday Set Null"/><rubicon:webbutton id=BirthdayTestSetNewValueButton runat="server" width="220px" Text="Birthday Set New Value"/></p>
      <p>Birthday Field Date Time Changed Label: <asp:label id=BirthdayFieldDateTimeChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
      <p><br><rubicon:webbutton id=ReadOnlyBirthdayTestSetNullButton runat="server" width="220px" Text="Read Only Birthday Set Null"/><rubicon:webbutton id=ReadOnlyBirthdayTestSetNewValueButton runat="server" width="220px" Text="Read Only Birthday Set New Value"/></p>
