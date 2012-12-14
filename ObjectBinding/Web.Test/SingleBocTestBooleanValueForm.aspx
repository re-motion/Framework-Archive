<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="SingleBocTestBooleanValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocBooleanValueForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Unit: TestBooleanValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: BooleanValue Form</h1>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocBooleanValue id="DeceasedField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="300px" nullitemerrormessage="Eingabe erforderlich" falsedescription="nein" nulldescription="undefiniert" truedescription="ja"></obc:BocBooleanValue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id="DeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocbooleanvalue id="ReadOnlyDeceasedField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="300px" readonly="True"></obc:bocbooleanvalue></td>
    <td>bound, read only</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocbooleanvalue id=UnboundDeceasedField runat="server" Width="150px" required="False" showdescription="False"></obc:bocbooleanvalue></td>
    <td>unbound, value not set, required= false, description=false</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocbooleanvalue id=UnboundReadOnlyDeceasedField runat="server" Width="150px" ReadOnly="True" height="8px"></obc:bocbooleanvalue></td>
    <td>unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyDeceasedFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p>
<p>Deceased Field Checked Changed: <asp:Label id="DeceasedFieldCheckedChangedLabel" runat="server" enableviewstate="False">#</asp:Label></p>
<p><asp:button id="DeceasedTestSetNullButton" runat="server" Text="Deceased Set Null" width="220px"></asp:button><asp:button id="DeceasedTestToggleValueButton" runat="server" Text="Deceased Toggle Value" width="220px"></asp:button></p>
<p><asp:button id="ReadOnlyDeceasedTestSetNullButton" runat="server" Text="Read Only Deceased Set Null" width="220px"></asp:button><asp:button id="ReadOnlyDeceasedTestToggleValueButton" runat="server" Text="Read Only Deceased Toggle Value" width="220px"></asp:button></p>
<p><rwc:FormGridManager id="FormGridManager" runat="server" visible="true"></rwc:FormGridManager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
<p>&nbsp;</p></form>
  </body>
</html>
