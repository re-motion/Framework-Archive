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
<form id=Form1 method=post runat="server"><rubicon:validationstateviewer id=ValidationStateViewer runat="server" visible="true"></rubicon:validationstateviewer><rubicon:formgridmanager id=GlobalFormGridManager runat="server" visible="true"></rubicon:formgridmanager>
<table id=MainFormGrid cellSpacing=0 cellPadding=0 
runat="server">
  <tr>
    <td colSpan=2><asp:label id=PersonDataLabel runat="server"></asp:label></TD></TR>
  <tr>
    <td><asp:label id=NameLabel runat="server" AssociatedControlID="NameField">&Name Local</asp:label></TD>
    <td><asp:textbox id=NameField runat="server"></asp:textbox><asp:comparevalidator id=NameFieldCompareValidator runat="server" ErrorMessage='Bitte geben Sie als Name "Foo Bar" an.' ControlToValidate="NameField" valuetocompare="Foo Bar"></asp:comparevalidator></TD></TR>
  <tr>
    <td><asp:Label id="CombinedName_Label" runat="server">Name</asp:Label></TD>
    <td>Vorname<asp:textbox id=FirstNameField runat="server" Width="64px"></asp:textbox> 
      Nachname<asp:textbox id=LastNameField runat="server" Width="134px"></asp:textbox></TD></TR>
  <tr>
    <td></TD>
    <td><asp:dropdownlist id=GenderList runat="server">
<asp:listitem Value="female">w</asp:listitem>
<asp:listitem Value="male">m</asp:listitem></asp:dropdownlist><asp:comparevalidator id=GenderListCompareValidator runat="server" ErrorMessage="Bitte wählen Sie männlich (m) aus." ControlToValidate="GenderList" ValueToCompare="male"></asp:comparevalidator></TD></TR>
  <tr>
    <td></TD>
    <td><asp:textbox id=ZipField runat="server"></asp:textbox><asp:textbox id=PlaceField runat="server"></asp:textbox></TD></TR>
  <tr>
    <td><rubicon:formgridlabel id=AdressFormGridLabel runat="server" HelpUrl="help.html" Required="True">Adresse</rubicon:formgridlabel></TD>
    <td><asp:textbox id=AddressField runat="server" ReadOnly="True">Die Adresse ist Read-Only</asp:textbox></TD></TR>
  <tr>
    <td><asp:label id=EducationLavel runat="server">Ausbildung</asp:label></TD>
    <td></TD></TR>
  <tr>
    <td colSpan=2><asp:textbox id=EducationField runat="server" width="100%" height="3em"></asp:textbox></TD></TR>
  <tr>
    <td></TD>
    <td></TD></TR>
  <tr>
    <td colSpan=2><asp:table id=CompaniesTable runat="server" width="100%" BorderStyle="Double" GridLines="Both" BorderColor="Blue">
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
</asp:table><asp:requiredfieldvalidator id=CompaniesTableRequiredFieldValidator runat="server" ErrorMessage="Bitte geben Sie eine Firma an." controltovalidate="Company1NameField"></asp:requiredfieldvalidator></TD></TR></TABLE><asp:button id=submitButton runat="server" Text="Abschicken"></asp:button></FORM>
  </body>
</html>
