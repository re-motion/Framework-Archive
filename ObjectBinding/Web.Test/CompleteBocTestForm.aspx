<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
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
<h1>CompleteBocTest: Form, No UserControl</H1>
<p>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:bocreferencevalue id=ReferenceField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<obc:BocCommand Type="None"></obc:BocCommand>
</PersistedCommand></obw:bocreferencevalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:bocmultilinetextvalue id=MultilineTextField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></obw:bocmultilinetextvalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:bocdatetimevalue id=DateTimeField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth"></obw:bocdatetimevalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:bocenumvalue id=EnumField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus"><listcontrolstyle 
      radiobuttonlistcellpadding="" 
      radiobuttonlistcellspacing=""></LISTCONTROLSTYLE></obw:bocenumvalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:boctextvalue id=TextField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="FirstName"></obw:boctextvalue></TD></TR>
  <tr>
    <td></TD>
    <td><obw:bocbooleanvalue id=BooleanField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased"></obw:bocbooleanvalue></TD></TR>
  <tr>
    <td></TD>
    <td></TD></TR>
  <tr>
    <td colSpan=2><obw:boclist id=ListField runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Jobs" showsortingorder="True" alwaysshowpageinfo="True" enableselection="True">
<fixedcolumns>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Title"></obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate"></obw:BocSimpleColumnDefinition>
</FixedColumns></obw:boclist></TD></TR></TABLE></P>
<p><rwc:formgridmanager id=FormGridManager runat="server" 
visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></P>
<p><asp:button id=SaveButton runat="server" Text="Save" Width="80px"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></P></FORM>
  </body>
</html>
