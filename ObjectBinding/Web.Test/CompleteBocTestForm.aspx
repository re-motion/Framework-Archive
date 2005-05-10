<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>CompleteBocTest: Form, No UserControl</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: Form, No UserControl</h1>
<p>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:boctextvalue id=TextField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="FirstName" errormessage="Fehler">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></obw:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocmultilinetextvalue id=MultilineTextField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37" errormessage="Fehler">
<textboxstyle textmode="MultiLine" autopostback="True">
</TextBoxStyle></obw:bocmultilinetextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocdatetimevalue id=DateTimeField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth" errormessage="Fehler">
<datetextboxstyle autopostback="True">
</DateTextBoxStyle></obw:bocdatetimevalue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><obw:bocenumvalue id=EnumField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" errormessage="Fehler">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obw:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=ReferenceField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner" errormessage="Fehler">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<obc:BocCommand Type="None"></obc:BocCommand>
</PersistedCommand></obw:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocbooleanvalue id=BooleanField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" errormessage="Fehler" autopostback="True"></obw:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obw:boclist id=ListField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Jobs" showsortingorder="True" alwaysshowpageinfo="True" enableselection="True">
<fixedcolumns>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="Title">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
</FixedColumns></obw:boclist></td></tr></table></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" 
visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></p>
<p><asp:button id=SaveButton runat="server" Text="Save" Width="80px"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p></form>
  </body>
</html>
