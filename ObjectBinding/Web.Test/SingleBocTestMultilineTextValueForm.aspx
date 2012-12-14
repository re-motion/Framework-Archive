<%@ Page language="c#" Codebehind="SingleBocTestMultilineTextValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocMultilineTextValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: TextValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: MultilineTextValue Form</h1>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:bocmultilinetextvalue id=CVField runat="server" required="True" PropertyIdentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" requirederrormessage="Eingabe erforderlich">
<textboxstyle rows="5" textmode="MultiLine">
</TextBoxStyle>
</obc:bocmultilinetextvalue></td>
    <td>
      <p>bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id="CVFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocmultilinetextvalue id=ReadOnlyCVField runat="server" PropertyIdentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" ReadOnly="True">
<textboxstyle rows="5" textmode="MultiLine">
</TextBoxStyle>
</obc:bocmultilinetextvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyCVFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocmultilinetextvalue id=UnboundCVField runat="server" Width="150px">
<textboxstyle rows="5" textmode="MultiLine">
</TextBoxStyle></obc:bocmultilinetextvalue></td>
    <td>
      <p>unbound, value not set, list-box, 
      required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundCVFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:bocmultilinetextvalue id=UnboundReadOnlyCVField runat="server" Width="150px" ReadOnly="True">
<textboxstyle rows="5" textmode="MultiLine">
</TextBoxStyle></obc:bocmultilinetextvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyCVFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr></table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button><br></p>
<p>CV Field Text Changed: <asp:label id="CVFieldTextChangedLabel" runat="server" EnableViewState="False">#</asp:label></p>
<p><asp:button id=CVTestSetNullButton runat="server" Text="VC Set Null" width="220px"></asp:button><asp:button id=CVTestSetNewValueButton runat="server" Text="CVSet New Value" width="220px"></asp:button></p>
<p><br><asp:button id=ReadOnlyCVTestSetNullButton runat="server" Text="Read Only CV Set Null" width="220px"></asp:button><asp:button id=ReadOnlyCVTestSetNewValueButton runat="server" Text="Read Only CV Set New Value" width="220px"></asp:button></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p></form>
  </body>
</html>
