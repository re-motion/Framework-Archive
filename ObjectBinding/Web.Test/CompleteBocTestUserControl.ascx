<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CompleteBocTestUserControl.ascx.cs" Inherits="OBWTest.CompleteBocUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>

<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>


<table id="FormGrid" runat="server">
  <tr>
    <td colspan="2"><obc:boctextvalue id="FirstNameField" runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></obc:boctextvalue>&nbsp;<obc:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocTextValue id="TextField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></obw:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocMultilineTextValue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></obw:BocMultilineTextValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocDateTimeValue id="DateTimeField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject"  incompleteerrormessage="Unvollständige Daten" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" invaliddateerrormessage="Ungültiges Datum" invalidtimeerrormessage="Ungültige Zeit" ></obw:BocDateTimeValue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><obw:BocEnumValue id="EnumField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obw:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocReferenceValue id="ReferenceField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject">
</obw:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><obw:BocBooleanValue id="BooleanField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject"></obw:BocBooleanValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colspan="2"><obw:BocList id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="CurrentObject" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True" selection="Multiple">
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
</FixedColumns></obw:BocList></td></tr></table>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:CurrentObject id="CurrentObject" runat="server" typename="OBRTest.Person, OBRTest"></obr:CurrentObject></p>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
<rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView" width="100%" height="10%">
<views> 
 <rwc:tabview id="first" title="First">
 </rwc:tabview>
 <rwc:tabview id="second" title="Second">
 </rwc:tabview>
</Views>
</rwc:tabbedmultiview>
