<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="PersonDetailsPage.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonDetailsPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>PersonDetailsPage</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="Html/style.css" type=text/css rel=stylesheet >
  </head>
<body>
<form id=Form method=post runat="server"><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2>Persondetails</td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px" required="True">
<textboxstyle cssclass="MyCssClass" autopostback="True">
</TextBoxStyle>
</obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px" required="True">
<textboxstyle cssclass="MyCssClass" autopostback="True">
</TextBoxStyle>
</obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocEnumValue id="GenderField" runat="server" datasource="<%# reflectionBusinessObjectDataSource %>" propertyidentifier="Gender">
<listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList" radiobuttonlistrepeatdirection="Vertical">
</ListControlStyle></obc:BocEnumValue></td></tr>
  <tr>
    <td style="HEIGHT: 14px"></td>
    <td style="HEIGHT: 14px"><obc:BocReferenceValue id="PartnerField" runat="server" datasource="<%# reflectionBusinessObjectDataSource %>" propertyidentifier="Partner"></obc:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocDateTimeValue id="BirthdayField" runat="server" datasource="<%# reflectionBusinessObjectDataSource %>" propertyidentifier="DateOfBirth"></obc:BocDateTimeValue></td></tr></table><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button><asp:Button id="NextButton" runat="server" Text="Next"></asp:Button></form>
	
  </body>
</html>
