<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="FormGrid.Test.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head id="HtmlHead" runat="server">
    <title>WebForm1</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><LINK href="Html/global.css" type=text/css rel=stylesheet >
  </head>
<body>
<form id=Form1 method=post runat="server"><rwc:FormGridManager id="GlobalFormGridManager" runat="server" visible="true" validatorvisibility="ValidationMessageAfterControlsColumn"></rwc:FormGridManager><rwc:ValidationStateViewer id="ValidationStateViewer1" runat="server" visible="true"></rwc:ValidationStateViewer>
<TABLE id=TableDesignTimeFormGrid cellSpacing=0 cellPadding=0 runat="server">
  <TR>
    <TD colSpan=5><asp:label id=PersonDataLabel runat="server">###</asp:label></TD></TR>
  <TR>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td>
    <TD><asp:label id=NameLabel runat="server" AssociatedControlID="NameField">&Name Local</asp:label></TD>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td>
    <TD><asp:textbox id=NameField runat="server"></asp:textbox><asp:comparevalidator id=CompareValidator1 runat="server" ErrorMessage='Please enter "Hello World" for a name.' ControlToValidate="NameField" valuetocompare="Hello World" tooltip='Name is not "Hello World"'></asp:comparevalidator></TD>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td></TR>
  <tr>
    <td style="BACKGROUND-COLOR: green"></td>
    <td><asp:Label id="ToBeHiddenLabel" runat="server">Label</asp:Label></td>
    <td style="BACKGROUND-COLOR: green"></td>
    <td><asp:TextBox id="ToBeHiddenTextBox" runat="server"></asp:TextBox></td>
    <td style="BACKGROUND-COLOR: green"></td></tr>
  <TR>
    <td style="BACKGROUND-COLOR: green">&nbsp;</td>
    <td></td>
    <td style="BACKGROUND-COLOR: green">&nbsp;</td>
    <TD><asp:dropdownlist id=GenderList runat="server" >
<asp:ListItem Value="female">w</asp:ListItem>
<asp:ListItem Value="male">m</asp:ListItem></asp:dropdownlist><asp:comparevalidator id=CompareValidator2 runat="server" ErrorMessage="Please select male." ControlToValidate="GenderList" ValueToCompare="male"></asp:comparevalidator><asp:comparevalidator id=Comparevalidator4 runat="server" ErrorMessage="Please select male. 2nd Validator just to reinforce that this is really important. ;)" ControlToValidate="GenderList" ValueToCompare="male"></asp:comparevalidator></TD>
    <td style="BACKGROUND-COLOR: green">&nbsp;</td></TR>
  <TR>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td>
    <TD></TD>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td>
    <TD>
      <P><asp:textbox id=ZipField runat="server"></asp:textbox><asp:textbox id=PlaceField runat="server"></asp:textbox></P></TD>
    <td style="BACKGROUND-COLOR: aqua">&nbsp;</td></TR>
  <TR>
    <td style="BACKGROUND-COLOR: red">&nbsp;</td>
    <TD><rwc:FormGridLabel id="AddressLabel" runat="server" AssociatedControlID="AddressField"></rwc:FormGridLabel></TD>
    <td style="BACKGROUND-COLOR: red">&nbsp;</td>
    <TD><asp:textbox id=AddressField runat="server" ReadOnly="True">Die Adresse ist Read-Only</asp:textbox></TD>
    <td style="BACKGROUND-COLOR: red">&nbsp;</td></TR>
  <TR>
    <td style="BACKGROUND-COLOR: blue">&nbsp;</td>
    <TD></TD>
    <td style="BACKGROUND-COLOR: blue">&nbsp;</td>
    <TD></TD>
    <td style="BACKGROUND-COLOR: blue">&nbsp;</td></TR>
  <TR>
    <td style="BACKGROUND-COLOR: yellow">&nbsp;</td>
    <TD class="mytest" colSpan=3><asp:table id=SomeBigMultiFieldThing runat="server" width="100%" BorderStyle="Double" GridLines="Both" BorderColor="Blue">
<asp:TableRow>
<asp:TableCell Text="Firma"></asp:TableCell>
<asp:TableCell Text="von"></asp:TableCell>
<asp:TableCell Text="bis"></asp:TableCell>
</asp:TableRow>
</asp:table><asp:comparevalidator id=CompareValidator3 runat="server" ErrorMessage="Bitte eine Firma eintragen" ValueToCompare="false" controltovalidate="TextBox2"></asp:comparevalidator></TD>
    <td style="BACKGROUND-COLOR: yellow">&nbsp;</td></TR>
  <TR>
    <td style="BACKGROUND-COLOR: gray">&nbsp;</td>
    <TD><rwc:FormGridLabel id="FormGridLabel" runat="server" HelpUrl="help.html" Required="True">Name</rwc:FormGridLabel></TD>
    <td style="BACKGROUND-COLOR: gray">&nbsp;</td>
    <TD>Vorname<asp:textbox id=TextBox1 runat="server" Width="64px"></asp:textbox> 
      Nachname<asp:textbox id=TextBox2 runat="server" Width="134px"></asp:textbox></TD>
        <td style="BACKGROUND-COLOR: gray">&nbsp;</td></TR>
  <tr>
    <td style="BACKGROUND-COLOR: gray"></td>
    <td></td>
    <td style="BACKGROUND-COLOR: gray"></td>
    <td></td>
    <td style="BACKGROUND-COLOR: gray"></td></tr></TABLE>
<P>Test</P><input type="text" value="Test">
<TABLE id=TableRunTime cellSpacing=1 cellPadding=1 width=456 border=1 runat="server">
  <TR>
    <TD></TD></TR>
  <TR>
    <TD style="WIDTH: 89px; HEIGHT: 23px" colSpan=3 
      >Personendaten</TD></TR>
  <TR>
    <TD style="WIDTH: 148px; HEIGHT: 25px" vAlign=top bgColor=aliceblue 
    >Name</TD>
    <TD style="WIDTH: 23px; HEIGHT: 25px" vAlign=top>!</TD>
    <TD style="HEIGHT: 25px"><INPUT type=text 
      ><FONT color=#ff0066> <BR 
      >Bitte geben Sie einen Wert für das Feld "Name" 
      ein.</FONT></TD></TR>
  <TR style="BORDER-TOP: medium none">
    <TD style="WIDTH: 148px; HEIGHT: 23px"></TD>
    <TD style="WIDTH: 23px; HEIGHT: 23px"></TD>
    <TD style="HEIGHT: 23px"></TD></TR>
  <TR>
    <TD style="WIDTH: 148px; HEIGHT: 25px">Geschlecht</TD>
    <TD style="WIDTH: 23px; HEIGHT: 25px">*</TD>
    <TD style="HEIGHT: 25px"><asp:dropdownlist id=DropDownList2 runat="server">
<asp:ListItem Value="1">1</asp:ListItem>
<asp:ListItem Value="2">2</asp:ListItem></asp:dropdownlist></TD></TR>
  <TR>
    <TD style="WIDTH: 148px">PLZ Ort</TD>
    <TD style="WIDTH: 23px"></TD>
    <TD><asp:textbox id=TextBox4 runat="server"></asp:textbox><asp:textbox id=TextBox5 runat="server"></asp:textbox></TD></TR>
  <TR>
    <TD style="WIDTH: 148px">Anstellungsverhältnisse</TD>
    <TD>&nbsp;&nbsp;&nbsp; ?</TD>
    <TD></TD></TR>
  <TR>
    <TD colSpan=3>
      <TABLE id=Table1 borderColor=#3300cc cellSpacing=1 cellPadding=1 width=300 
      border=1>
        <TR>
          <TD style="WIDTH: 123px">Firma</TD>
          <TD>Von</TD>
          <TD>Bis</TD></TR>
        <TR>
          <TD style="WIDTH: 123px">F1</TD>
          <TD></TD>
          <TD></TD></TR>
        <TR>
          <TD style="WIDTH: 123px">F2</TD>
          <TD></TD>
          <TD></TD></TR></TABLE><FONT color=#ff3366 
      >Bitte eine dritte Firma</FONT> </TD></TR></TABLE><asp:button id=Button1 runat="server" Text="Post Back"></asp:button></form>
	
  </body>
</html>
