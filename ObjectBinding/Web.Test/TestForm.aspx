<%@ Page language="c#" Codebehind="WebFormMK.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebFormMK" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>WebFormMK</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="Html/style.css" type=text/css rel=stylesheet >
  </head>
<body>
<form id=Form1 method=post runat="server"><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2>Person</td></tr>
  <tr>
    <td></td>
    <td><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px" required="True">
<textboxstyle cssclass="MyCssClass" autopostback="True">
</TextBoxStyle>
</obc:boctextvalue><obc:BocTextValueValidator id="BocTextValueValidator1" runat="server" ControlToValidate="FirstNameField"></obc:BocTextValueValidator></td></tr>
  <tr>
    <td></td>
    <td><obc:BocTextValue id="BirthdayField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource %>" PropertyIdentifier="DateOfBirth"></obc:BocTextValue><obc:BocTextValueValidator id="BocTextValueValidator2" runat="server" ControlToValidate="BirthdayField"></obc:BocTextValueValidator></td></tr></table><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></form>
	
  </body>
</html>
