<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CompleteBocTestUserControl.ascx.cs" Inherits="OBWTest.CompleteBocUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>






<table id="FormGrid" runat="server">
  <tr>
    <td colspan="2"><rubicon:boctextvalue id="FirstNameField" runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocTextValue id="TextField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></rubicon:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocMultilineTextValue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></rubicon:BocMultilineTextValue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocDateTimeValue id="DateTimeField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject"  incompleteerrormessage="Unvollständige Daten" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" invaliddateerrormessage="Ungültiges Datum" invalidtimeerrormessage="Ungültige Zeit" ></rubicon:BocDateTimeValue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><rubicon:BocEnumValue id="EnumField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocReferenceValue id="ReferenceField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject">
</rubicon:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocBooleanValue id="BooleanField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject"></rubicon:BocBooleanValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colspan="2"><rubicon:BocList id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="CurrentObject" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True" selection="Multiple">
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
</FixedColumns></rubicon:BocList></td></tr></table>
<p><rubicon:formgridmanager id="FormGridManager" runat="server" visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Rubicon.ObjectBinding.Sample::Person"></rubicon:BindableObjectDataSourceControl></p>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
<rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView" width="100%" height="10%">
<views> 
 <rubicon:tabview id="first" title="First">
 </rubicon:tabview>
 <rubicon:tabview id="second" title="Second">
 </rubicon:tabview>
</Views>
</rubicon:tabbedmultiview>
