<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="SingleBocTestDateTimeValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocDateTimeValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: DateTimeValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: DateTimeValue Form</h1>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obc:boctextvalue id=FirstNameField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="FirstName" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="LastName" ReadOnly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=BirthdayField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="DateOfBirth" showseconds="False" readonly="False" width="300px" incompleteerrormessage="Unvollständige Daten" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" invaliddateerrormessage="Ungültiges Datum" invalidtimeerrormessage="Ungültige Zeit" requirederrormessage="Eingabe erforderlich"></obc:bocdatetimevalue></td>
    <td>
      <p>bound</p></td>
    <td style="WIDTH: 20%"><asp:label id="BirthdayFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=ReadOnlyBirthdayField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="DateOfBirth" showseconds="False" readonly="True"></obc:bocdatetimevalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyBirthdayFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=UnboundBirthdayField runat="server" readonly="False" width="300px" required="False"></obc:bocdatetimevalue></td>
    <td>
      <p>unbound, value not set, not required</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundBirthdayFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>      
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=UnboundReadOnlyBirthdayField runat="server" readonly="True"></obc:bocdatetimevalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyBirthdayFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=DateOfDeathField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="DateOfDeath" readonly="False" width="300px"></obc:bocdatetimevalue></td>
    <td>
      <p>bound</p></td>
    <td style="WIDTH: 20%"><asp:label id="DateOfDeathFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=ReadOnlyDateOfDeathField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="DateOfDeath" readonly="True"></obc:bocdatetimevalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDateOfDeathFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=UnboundDateOfDeathField runat="server" readonly="False" required="False"></obc:bocdatetimevalue></td>
    <td>
      <p>unbound, value not set, not required</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDateOfDeathFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocdatetimevalue id=UnboundReadOnlyDateOfDeathField runat="server" readonly="True"></obc:bocdatetimevalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDateOfDeathFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td>Today</td>
    <td><obc:bocdatetimevalue id=DirectlySetBocDateTimeValueField runat="server" readonly="False" required="False" valuetype="Date"></obc:bocdatetimevalue></td>
    <td>
      <p>directly set, not required</p></td>
    <td style="WIDTH: 20%"><asp:label id="DirectlySetBocDateTimeValueFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td>Today</td>
    <td><obc:bocdatetimevalue id=ReadOnlyDirectlySetBocDateTimeValueField runat="server" readonly="True" valuetype="Date"></obc:bocdatetimevalue></td>
    <td>
      <p>directly set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
</table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button><br></p>
<p><asp:button id=BirthdayTestSetNullButton runat="server" width="220px" Text="Birthday Set Null"></asp:button><asp:button id=BirthdayTestSetNewValueButton runat="server" width="220px" Text="Birthday Set New Value"></asp:button></p>
<p>Birthday Field Date Time Changed Label: <asp:label id="BirthdayFieldDateTimeChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><br><asp:button id=ReadOnlyBirthdayTestSetNullButton runat="server" width="220px" Text="Read Only Birthday Set Null"></asp:button><asp:button id=ReadOnlyBirthdayTestSetNewValueButton runat="server" width="220px" Text="Read Only Birthday Set New Value"></asp:button></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" 
visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></p></form>
  </body>
</html>
