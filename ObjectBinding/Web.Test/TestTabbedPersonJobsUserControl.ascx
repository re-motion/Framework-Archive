<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonJobsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonJobsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td><rubicon:bocmultilinetextvalue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></rubicon:bocmultilinetextvalue></td>
  </tr>
  <tr>
    <td></td>
    <td></td>
  </tr>
  <tr>
    <td colspan="2"><rubicon:boclist id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True">
<fixedcolumns>
<rubicon:bocsimplecolumndefinition PropertyPathIdentifier="Title"></rubicon:bocsimplecolumndefinition>
<rubicon:bocsimplecolumndefinition PropertyPathIdentifier="StartDate"></rubicon:bocsimplecolumndefinition>
<rubicon:bocsimplecolumndefinition PropertyPathIdentifier="EndDate"></rubicon:bocsimplecolumndefinition>
</FixedColumns></rubicon:boclist></td>
</tr>
</table>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
