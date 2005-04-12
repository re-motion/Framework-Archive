<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonJobsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonJobsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>


<table id="FormGrid" runat="server" style="margin-top: 80%">
  <tr>
    <td></td>
    <td><rubicon:bocmultilinetextvalue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></rubicon:bocmultilinetextvalue></td>
  </tr>
  <tr>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td colspan="2"><rubicon:boclist id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True" pagesize="1">
<listmenuitems>
<rubicon:BocMenuItem Text="Copy" ItemID="Copy">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
<rubicon:BocMenuItem Text="Paste" ItemID="Paste">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
</ListMenuItems>

<fixedcolumns>
<rubicon:BocEditDetailsColumnDefinition SaveText="Save" CancelText="Cancel" ColumnID="EditDetails" EditText="Edit"></rubicon:BocEditDetailsColumnDefinition>
<rubicon:BocCommandColumnDefinition Text="Event" ColumnID="Event">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event"></rubicon:BocListItemCommand>
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
</FixedColumns>

<optionsmenuitems>
<rubicon:BocMenuItem Text="Copy" ItemID="Copy">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
<rubicon:BocMenuItem Text="Paste" ItemID="Paste">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
</OptionsMenuItems></rubicon:boclist></td>
</tr>
</table>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
