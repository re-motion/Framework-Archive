<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
<%@ Page language="c#" Codebehind="WebForm.aspx.cs" AutoEventWireup="false" Inherits="FormGrid.Sample.WebForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>WebForm1</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="Html/style.css" type=text/css rel=stylesheet >
  </head>
  <body >
	
    <form id="Form1" method="post" runat="server"><rubicon:ValidationStateViewer id="ValidationStateViewer" runat="server" visible="true"></rubicon:ValidationStateViewer><rubicon:FormGridManager id="GlobalFormGridManager" runat="server" visible="true"></rubicon:FormGridManager>
    
<table id=MainFormGrid cellSpacing=0 cellPadding=0 runat="server">
  <tr>
    <td colSpan=5><asp:label id=PersonDataLabel runat="server"></asp:label></td></tr>
  <tr>
    <td><asp:label id=NameLabel runat="server" AssociatedControlID="NameField">&Name Local</asp:label></td>
    <td><asp:textbox id=NameField runat="server">&amp;Name</asp:textbox><asp:comparevalidator id=NameFieldCompareValidator runat="server" ErrorMessage='Please enter "Hello World" for a name.' ControlToValidate="NameField" valuetocompare="Hello World" tooltip='Name is not "Hello World"'></asp:comparevalidator></td></tr>
  <tr>
    <td></td>
    <td><asp:dropdownlist id=GenderList runat="server" >
<asp:listitem Value="female">w</asp:listitem>
<asp:listitem Value="male">m</asp:listitem></asp:dropdownlist><asp:comparevalidator id=GenderListCompareValidator runat="server" ErrorMessage="Please select male." ControlToValidate="GenderList" ValueToCompare="male"></asp:comparevalidator></td></tr>
  <tr>
    <td></td>
    <td>
      <p><asp:textbox id=ZipField runat="server">&amp;Plz</asp:textbox><asp:textbox id=PlaceField runat="server">&amp;Ort</asp:textbox></p></td></tr>
  <tr>
    <td><asp:label id=AddressLabel runat="server">Address</asp:label></td>
    <td><asp:textbox id=AddressField runat="server" ReadOnly="True">Die Adresse ist Read-Only</asp:textbox></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=3><asp:table id=CompaniesTable runat="server" width="100%" BorderStyle="Double" GridLines="Both" BorderColor="Blue">
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
</asp:table><asp:RequiredFieldValidator id="CompaniesTableRequiredFieldValidator" runat="server" ErrorMessage="Bitte eine Firma eingeben" controltovalidate="Company1NameField"></asp:RequiredFieldValidator></td></tr>
  <tr>
    <td><rubicon:formgridlabel id="FormGridLabel" runat="server" HelpUrl="help.html" Required="True">Name</rubicon:formgridlabel></td>
    <td>Vorname<asp:textbox id=TextBox1 runat="server" Width="64px"></asp:textbox> 
      Nachname<asp:textbox id=TextBox2 runat="server" Width="134px"></asp:textbox></td>
        
</tr></table><asp:Button id="submitButton" runat="server" Text="Abschicken"></asp:Button>
    </form>	
  </body>
</html>
