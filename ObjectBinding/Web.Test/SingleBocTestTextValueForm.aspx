<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="SingleBocTestTextValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocTextValueForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTextValueForm</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: TextValue Form</h1>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4>Person</td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=FirstNameField runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="FirstName" required="True"></obc:boctextvalue></td>
    <td>
      <p>bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id="FirstNameFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
    </tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=ReadOnlyFirstNameField runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="FirstName" ReadOnly="True"></obc:boctextvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyFirstNameFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=UnboundFirstNameField runat="server" Width="150px"></obc:boctextvalue></td>
    <td>
      <p>unbound, value not set, list-box, required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundFirstNameFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=UnboundReadOnlyFirstNameField runat="server" Width="150px" ReadOnly="True"></obc:boctextvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyFirstNameFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button><br></p>
<p><asp:button id=FirstNameTestSetNullButton runat="server" Text="FirstName Set Null" width="220px"></asp:button><asp:button id=FirstNameTestSetNewValueButton runat="server" Text="FirstName Set New Value" width="220px"></asp:button></p>
<p>FirstName Field Text Changed: <asp:label id="FirstNameFieldTextChangedLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p><br><asp:button id=ReadOnlyFirstNameTestSetNullButton runat="server" Text="Read Only FirstName Set Null" width="220px"></asp:button><asp:button id=ReadOnlyFirstNameTestSetNewValueButton runat="server" Text="Read Only FirstName Set New Value" width="220px"></asp:button></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p></form>
  </body>
</html>
