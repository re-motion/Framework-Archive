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
    <td><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px">
<textboxstyle autopostback="True" cssclass="MyCssClass">
</TextBoxStyle>
</obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:smartlabel id=SmartLabel2 runat="server" ForControl="LastNameField"></rwc:smartlabel></td>
    <td><obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px"></obc:boctextvalue></td></tr>
  <tr>
    <td><rwc:smartlabel id=SmartLabel3 runat="server" ForControl="DateOfBirthField"></rwc:smartlabel></td>
    <td><obc:boctextvalue id=DateOfBirthField runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# reflectionBusinessObjectDataSource %>" ValueType="Date" Width="200px"></obc:boctextvalue><obc:boctextvaluevalidator id=BocTextValueValidator1 runat="server" EnableClientScript="False" ControlToValidate="DateOfBirthField"></obc:boctextvaluevalidator></td></tr>
  <tr>
    <td><rwc:smartlabel id=SmartLabel4 runat="server" ForControl="HeightField"></rwc:smartlabel></td>
    <td><obc:boctextvalue id=HeightField runat="server" PropertyIdentifier="Height" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px"></obc:boctextvalue><obc:boctextvaluevalidator id=BocTextValueValidator2 runat="server" EnableClientScript="False" ControlToValidate="HeightField"></obc:boctextvaluevalidator></td></tr>
  <tr>
    <td><rwc:smartlabel id=SmartLabel5 runat="server" ForControl="GenderField"></rwc:smartlabel></td>
    <td><obc:bocenumvalue id=GenderField runat="server" PropertyIdentifier="Gender" DataSource="<%# reflectionBusinessObjectDataSource %>" Height="24px" Width="152px"><listcontrolstyle font-bold="True" bordercolor="Red" 
      forecolor="Green" backcolor="#FFFF80" radiobuttonlisttextalign="Right" 
      radionbuttonlistrepeatlayout="Table" controltype="RadioButtonList" 
      radiobuttonlistrepeatdirection="Vertical"></LISTCONTROLSTYLE></obc:bocenumvalue></td></tr>
  <tr>
    <td><rwc:smartlabel id=SmartLabel6 runat="server" ForControl="MarriageStatusField"></rwc:smartlabel></td>
    <td><obc:bocenumvalue id=MarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" DataSource="<%# reflectionBusinessObjectDataSource %>" Width="200px">
<listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList" radiobuttonlistrepeatdirection="Vertical">
</ListControlStyle>
</obc:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><listcontrolstyle 
      radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" 
      controltype="DropDownList" radiobuttonlistrepeatdirection="Vertical"><obc:BocReferenceValue id="PartnerField" runat="server" width="200px" datasource="<%# reflectionBusinessObjectDataSource %>" propertyidentifier="Partner" enableicon="True"></obc:BocReferenceValue></listcontrolstyle></td></tr></table><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button><asp:button id=TestSetNullButton runat="server" Text="Set Null"></asp:button><asp:button id="TestSetNewItemButton" runat="server" Text="Set New Item"></asp:button><asp:button id="TestReadValueButton" runat="server" Text="Read Value"></asp:button><asp:Label id="ReadValueLabel" runat="server">not set</asp:Label></form>
	
  </body>
</html>
