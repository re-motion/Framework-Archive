<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Integration Test: Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: Form, No UserControl</h1>
<p>
<table id="FormGrid" runat="server">
  <tr>
    <td colspan="2"><obc:boctextvalue id="FirstNameField" runat="server" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="FirstName"></obc:boctextvalue>&nbsp;<obc:boctextvalue id="LastNameField" runat="server" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="LastName"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocTextValue id="TextField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="FirstName"></obw:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocMultilineTextValue id="MultilineTextField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></obw:BocMultilineTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocDateTimeValue id="DateTimeField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth"></obw:BocDateTimeValue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><obw:BocEnumValue id="EnumField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus"><listcontrolstyle 
      radiobuttonlistcellpadding="" 
      radiobuttonlistcellspacing=""></LISTCONTROLSTYLE></obw:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocReferenceValue id="ReferenceField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner"></obw:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocBooleanValue id="BooleanField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased"></obw:BocBooleanValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colspan="2"><obw:BocList id="ListField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Jobs" enableselection="True" alwaysshowpageinfo="True" showsortingorder="True">
<fixedcolumns>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Title"></obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate"></obw:BocSimpleColumnDefinition>
</FixedColumns></obw:BocList></td></tr></table></p>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBWTest.Person, OBWTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p></form>
  </body>
</html>
