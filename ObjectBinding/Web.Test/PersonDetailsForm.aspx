<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="PersonDetailsPage.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonDetailsPage" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head id="HtmlHeader" runat="server">
    <title>PersonDetailsPage</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
  </head>
<body>
<form id=Form method=post runat="server">
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2>Persondetails</td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="200px" required="True">
<textboxstyle cssclass="MyCssClass" autopostback="True">
</TextBoxStyle>
</obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="200px" required="True">
<textboxstyle cssclass="MyCssClass" autopostback="True">
</TextBoxStyle>
</obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocEnumValue id="GenderField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Gender">
<listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList" radiobuttonlistrepeatdirection="Vertical">
</ListControlStyle></obc:BocEnumValue></td></tr>
  <tr>
    <td style="HEIGHT: 14px"></td>
    <td style="HEIGHT: 14px"><obc:BocReferenceValue id="PartnerField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner"></obc:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocDateTimeValue id="BirthdayField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth"></obc:BocDateTimeValue></td></tr></table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button><asp:Button id="NextButton" runat="server" Text="Next"></asp:Button></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBWTest.Person, OBWTest"></obr:ReflectionBusinessObjectDataSourceControl></p></form>
	
  </body>
</html>
