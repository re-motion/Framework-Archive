<%@ Page language="c#" Codebehind="WebFormMK.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebFormMK" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>WebFormMK</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="Html/style.css" type=text/css rel=stylesheet >
  </head>
  <body >
	
    <form id="Form1" method="post" runat="server"><rwc:FormGridManager id="FormGridManager" runat="server" visible="true"></rwc:FormGridManager>
<table id="FormGrid" runat="server">
  <tr>
    <td  colspan="2">Person</td></tr>
  <tr>
    <td><rwc:SmartLabel id="BocPropertyLabel1" runat="server" ForControl="FirstNameField"></rwc:SmartLabel></td>
    <td><obc:boctextvalue id="FirstNameField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="FirstName"> <textboxstyle cssclass="MyCssClass" 
      autopostback="True"></TEXTBOXSTYLE></obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:SmartLabel id="BocPropertyLabel2" runat="server" ForControl="LastNameField"></rwc:SmartLabel></td>
    <td><obc:boctextvalue id="LastNameField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="LastName"> </obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:SmartLabel id="BocPropertyLabel3" runat="server" ForControl="DateOfBirthField"></rwc:SmartLabel></td>
    <td><obc:boctextvalue id="DateOfBirthField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="DateOfBirth" ValueType="Date"> </obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:SmartLabel id="BocPropertyLabel4" runat="server" ForControl="HeightField"></rwc:SmartLabel></td>
    <td><obc:boctextvalue id="HeightField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="Height"> </obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:SmartLabel id="BocPropertyLabel5" runat="server" ForControl="GenderField"></rwc:SmartLabel></td>
    <td><obc:BocEnumValue id="GenderField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="Gender" Width="152px" Height="24px"> <listcontrolstyle 
      radiobuttonlistrepeatdirection="Vertical" controltype="RadioButtonList" 
      radionbuttonlistrepeatlayout="Table" radiobuttonlisttextalign="Right" 
      backcolor="#FFFF80" forecolor="Green" bordercolor="Red" 
      font-bold="True"></LISTCONTROLSTYLE></obc:BocEnumValue></td></tr>
  <tr>
    <td><rwc:SmartLabel id="SmartLabel1" runat="server" ForControl="MarriageStatusField"></rwc:SmartLabel></td>
    <td><obc:BocEnumValue id="MarriageStatusField" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="MarriageStatus" Width="152px"> <listcontrolstyle 
      radiobuttonlistrepeatdirection="Vertical" controltype="DropDownList" 
      radionbuttonlistrepeatlayout="Table" 
      radiobuttonlisttextalign="Right"></LISTCONTROLSTYLE></obc:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr></table><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button>

     </form>
	
  </body>
</html>
