<%@ Control Language="c#" AutoEventWireup="false" Codebehind="IndependentUserControl.ascx.cs" Inherits="OBWTest.IndependentUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>


<table id="FormGrid" runat="server">
  <tr>
    <td colspan="2"><obc:boctextvalue id="FirstNameField" runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" ReadOnly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocTextValue id="TextField" runat="server" propertyidentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obw:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocMultilineTextValue id="MultilineTextField" runat="server" DESIGNTIMEDRAGDROP="37" propertyidentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obw:BocMultilineTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocDateTimeValue id="DateTimeField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obw:BocDateTimeValue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><obw:BocEnumValue id="EnumField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obw:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocReferenceValue id="ReferenceField" runat="server" propertyidentifier="Partner" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obw:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocBooleanValue id="BooleanField" runat="server" propertyidentifier="Deceased" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obw:BocBooleanValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colspan="2"><obw:BocList id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<fixedcolumns>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="Title"></obc:BocSimpleColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate"></obc:BocSimpleColumnDefinition>
</FixedColumns></obw:BocList></td></tr></table>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBWTest.Person, OBWTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
