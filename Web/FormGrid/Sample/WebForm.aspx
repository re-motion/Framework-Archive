<%@ Page language="c#" Codebehind="WebForm.aspx.cs" AutoEventWireup="false" Inherits="FormGrid.Sample.WebForm" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>WebForm1</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="Html/style.css" type=text/css rel=stylesheet >
  </head>
<body>
<form id=Form1 method=post runat="server"><rubicon:validationstateviewer id=ValidationStateViewer runat="server" visible="true"></rubicon:ValidationStateViewer><rubicon:formgridmanager id=GlobalFormGridManager runat="server" visible="true"></rubicon:FormGridManager>
<table id=MainFormGrid cellSpacing=0 cellPadding=0 
runat="server">
  <tr>
    <td colSpan=5><asp:label id=PersonDataLabel runat="server"></asp:label></TD></TR>
  <tr>
    <td><asp:label id=NameLabel runat="server" AssociatedControlID="NameField">&Name Local</asp:label></TD>
    <td><asp:textbox id=NameField runat="server">&amp;Name</asp:textbox><asp:comparevalidator id=NameFieldCompareValidator runat="server" valuetocompare="Foo Bar" ControlToValidate="NameField" ErrorMessage='Bitte geben Sie als Name "Foo Bar" an.'></asp:comparevalidator></TD></TR>
  <tr>
    <td></TD>
    <td><asp:dropdownlist id=GenderList runat="server">
<asp:listitem Value="female">w</asp:listitem>
<asp:listitem Value="male">m</asp:listitem></asp:dropdownlist><asp:comparevalidator id=GenderListCompareValidator runat="server" ControlToValidate="GenderList" ErrorMessage="Bitte wählen Sie männlich (m) aus." ValueToCompare="male"></asp:comparevalidator></TD></TR>
  <tr>
    <td></TD>
    <td>
      <p><asp:textbox id=ZipField runat="server"></asp:textbox><asp:textbox id=PlaceField runat="server"></asp:textbox></P></TD></TR>
  <tr>
    <td><asp:label id=AddressLabel runat="server">Address</asp:label></TD>
    <td><asp:textbox id=AddressField runat="server" ReadOnly="True">Die Adresse ist Read-Only</asp:textbox></TD></TR>
  <tr>
    <td></TD>
    <td></TD></TR>
  <tr>
    <td colSpan=3><asp:table id=CompaniesTable runat="server" BorderColor="Blue" GridLines="Both" BorderStyle="Double" width="100%">
<asp:TableRow>
<asp:TableCell Text="Firma"></asp:TableCell>
<asp:TableCell Text="von"></asp:TableCell>
<asp:TableCell Text="bis"></asp:TableCell>
</asp:TableRow>
<asp:TableRow>
<asp:TableCell><asp:TextBox id="Company1NameField" runat="server"></asp:TextBox></asp:TableCell>
<asp:TableCell></asp:TableCell>
<asp:TableCell></asp:TableCell>
</asp:TableRow>
</asp:table><asp:requiredfieldvalidator id=CompaniesTableRequiredFieldValidator runat="server" ErrorMessage="Bitte geben Sie eine Firma an." controltovalidate="Company1NameField"></asp:RequiredFieldValidator></TD></TR>
  <tr>
    <td><rubicon:formgridlabel id=FormGridLabel runat="server" Required="True" HelpUrl="help.html">Name</rubicon:formgridlabel></TD>
    <td>Vorname<asp:textbox id=TextBox1 runat="server" Width="64px"></asp:textbox> 
      Nachname<asp:textbox id=TextBox2 runat="server" Width="134px"></asp:textbox></TD></TR></TABLE><asp:button id=submitButton runat="server" Text="Abschicken"></asp:Button></FORM>	
  </body>
</html>
