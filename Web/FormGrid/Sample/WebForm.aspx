<%@ Page language="c#" Codebehind="WebForm.aspx.cs" AutoEventWireup="false" Inherits="FormGrid.Sample.WebForm" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head id="HtmlHead" runat="server">
    <title>WebForm1</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
  </head>
<body>
<form id=Form1 method=post runat="server"><rwc:ValidationStateViewer id="ValidationStateViewer" runat="server" visible="true" DESIGNTIMEDRAGDROP="62"></rwc:ValidationStateViewer>
<table id=MainFormGrid cellSpacing=0 cellPadding=0 
runat="server">
  <tr>
    <td colSpan=2><asp:label id=PersonDataLabel runat="server"></asp:label></td></tr>
  <tr>
    <td><asp:label id=NameLabel runat="server" AssociatedControlID="NameField">&Name Local</asp:label></td>
    <td><asp:textbox id=NameField runat="server"></asp:textbox><asp:comparevalidator id=NameFieldCompareValidator runat="server" ErrorMessage='Bitte geben Sie als Name "Foo Bar" an.' ControlToValidate="NameField" valuetocompare="Foo Bar"></asp:comparevalidator></td></tr>
  <tr>
    <td><asp:Label id="CombinedName_Label" runat="server">Name</asp:Label></td>
    <td>Vorname<asp:textbox id=FirstNameField runat="server" Width="64px"></asp:textbox> 
      Nachname<asp:textbox id=LastNameField runat="server" Width="134px"></asp:textbox></td></tr>
  <tr>
    <td></td>
    <td><asp:dropdownlist id=GenderList runat="server">
<asp:listitem Value="female">w</asp:listitem>
<asp:listitem Value="male">m</asp:listitem></asp:dropdownlist><asp:comparevalidator id=GenderListCompareValidator runat="server" ErrorMessage="Bitte wählen Sie männlich (m) aus." ControlToValidate="GenderList" ValueToCompare="male"></asp:comparevalidator></td></tr>
  <tr>
    <td></td>
    <td><asp:textbox id=ZipField runat="server"></asp:textbox><asp:textbox id=PlaceField runat="server"></asp:textbox></td></tr>
  <tr>
    <td><rwc:FormGridLabel id="AddressLabel" runat="server" AssociatedControlID="AddressField"></rwc:FormGridLabel></td>
    <td><asp:textbox id=AddressField runat="server" ReadOnly="True">Die Adresse ist Read-Only</asp:textbox></td></tr>
  <tr>
    <td><asp:label id=EducationLavel runat="server">Ausbildung</asp:label></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><asp:textbox id=EducationField runat="server" width="100%" height="3em"></asp:textbox></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
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
</asp:table><asp:requiredfieldvalidator id=CompaniesTableRequiredFieldValidator runat="server" ErrorMessage="Bitte geben Sie eine Firma an." controltovalidate="Company1NameField"></asp:requiredfieldvalidator></td></tr></table><asp:button id=SubmitButton runat="server" Text="Abschicken"></asp:button><rwc:FormGridManager id="GlobalFormGridManager" runat="server" visible="true"></rwc:FormGridManager></form>
  </body>
</html>
