<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonJobsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonJobsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>





<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td><rubicon:bocmultilinetextvalue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject"></rubicon:bocmultilinetextvalue></td>
  </tr>
  <tr>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td colspan="2"><rubicon:boclist id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="CurrentObject" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True" selection="Multiple" >
<fixedcolumns>
<rubicon:BocRowEditModeColumnDefinition SaveText="Save" CancelText="Cancel" EditText="Edit"></rubicon:BocRowEditModeColumnDefinition>
<rubicon:BocCommandColumnDefinition Text="Event">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<rubicon:BocListItemCommand Type="Href" HrefCommand-Href="Start.aspx"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
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
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="EndDate">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
</FixedColumns></rubicon:boclist></td>
</tr>
</table>
<p><rubicon:formgridmanager id="FormGridManager" runat="server" visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" typename="Rubicon.ObjectBinding.Sample::Person"></rubicon:BindableObjectDataSourceControl></p>
