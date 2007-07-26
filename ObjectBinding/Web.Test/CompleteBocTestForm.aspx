



<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>CompleteBocTest: Form, No UserControl</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: Form, No UserControl</h1>
<p>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=TextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="FirstName" errormessage="Fehler">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocmultilinetextvalue id=MultilineTextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37" errormessage="Fehler">
<textboxstyle textmode="MultiLine" autopostback="True">
</TextBoxStyle></rubicon:bocmultilinetextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocdatetimevalue id=DateTimeField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="DateOfBirth" errormessage="Fehler">
<datetextboxstyle autopostback="True">
</DateTextBoxStyle></rubicon:bocdatetimevalue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><rubicon:bocenumvalue id=EnumField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" errormessage="Fehler">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocreferencevalue id=ReferenceField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" errormessage="Fehler">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<rubicon:BocCommand Type="None"></rubicon:BocCommand>
</PersistedCommand></rubicon:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id=BooleanField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" errormessage="Fehler" autopostback="True"></rubicon:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><rubicon:boclist id=ListField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Jobs" showsortingorder="True" alwaysshowpageinfo="True" enableselection="True">
<fixedcolumns>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Title">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
</FixedColumns></rubicon:boclist></td></tr></table></p>
<p><rubicon:formgridmanager id=FormGridManager runat="server" 
visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl 
id=CurrentObject runat="server" 
typename="Rubicon.ObjectBinding.Sample::Person" /></p>
<p><asp:button id=SaveButton runat="server" Text="Save" Width="80px"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p></form>
  </body>
</html>
