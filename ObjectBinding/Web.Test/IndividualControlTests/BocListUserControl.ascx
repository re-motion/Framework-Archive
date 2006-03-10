<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><obw:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue></td></tr>
  <tr>
    <td>Jobs</td>
    <td><obw:boclist id=JobList runat="server" datasourcecontrol="CurrentObject" showallproperties="True" showavailableviewslist="False" showsortingorder="True" propertyidentifier="Jobs" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="2" selection="SingleRadioButton" index="Disabled">
</obw:boclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obrt:testboclist id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple">
<fixedcolumns>
<obw:BocEditDetailsColumnDefinition ItemID="EditDetails" SaveText="Save" CancelText="Cancel" Width="2em" EditText="Edit"></obw:BocEditDetailsColumnDefinition>
<obw:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<obw:BocListItemCommand Type="Event" CommandStateType="OBRTest::PersonListItemCommandState" ToolTip="An Event Command"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCommandColumnDefinition>
<obw:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<obw:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCommandColumnDefinition>
<obw:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<obw:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<obw:PropertyPathBinding PropertyPathIdentifier="LastName"></obw:PropertyPathBinding>
<obw:PropertyPathBinding PropertyPathIdentifier="FirstName"></obw:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCompoundColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="4em" ColumnTitle="Partner">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<obw:BocListItemCommand Type="Event"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="OBRTest::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></obw:BocCustomColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></obw:BocDropDownMenuColumnDefinition>
</FixedColumns>
</obrt:testboclist></td></tr>
<%--  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obrt:testboclist id=Testboclist1 runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" indexoffset="100" ShowEmptyListMessage="True" enableselection="True" Index="SortedOrder" Selection="Multiple" rowmenudisplay="Automatic" readonly="True">
<fixedcolumns>
<obw:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<obw:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<obw:PropertyPathBinding PropertyPathIdentifier="LastName"></obw:PropertyPathBinding>
<obw:PropertyPathBinding PropertyPathIdentifier="FirstName"></obw:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCompoundColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="4em" ColumnTitle="Partner">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<obw:BocListItemCommand Type="Event"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
</FixedColumns>
</obrt:testboclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obrt:testboclist id=EmptyList runat="server" datasourcecontrol="EmptyDataSourceControl" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple" required="True" readonly="False">
<listmenuitems>
<obw:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<obw:BocMenuItemCommand Type="Event"></obw:BocMenuItemCommand>
</PersistedCommand>
</obw:BocMenuItem>
</ListMenuItems>

<optionsmenuitems>
<obw:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<obw:BocMenuItemCommand Type="Event"></obw:BocMenuItemCommand>
</PersistedCommand>
</obw:BocMenuItem>
</OptionsMenuItems>

<fixedcolumns>
<obw:BocEditDetailsColumnDefinition ItemID="EditDetails" SaveText="Save" CancelText="Cancel" EditText="Edit"></obw:BocEditDetailsColumnDefinition>
<obw:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<obw:BocListItemCommand Type="Event" CommandStateType="OBRTest::PersonListItemCommandState" ToolTip="An Event Command"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCommandColumnDefinition>
<obw:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<obw:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCommandColumnDefinition>
<obw:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<obw:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<obw:PropertyPathBinding PropertyPathIdentifier="LastName"></obw:PropertyPathBinding>
<obw:PropertyPathBinding PropertyPathIdentifier="FirstName"></obw:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCompoundColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" ColumnTitle="Partner">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<obw:BocListItemCommand Type="Event"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="OBRTest::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></obw:BocCustomColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></obw:BocDropDownMenuColumnDefinition>
</FixedColumns>
</obrt:testboclist><obrt:testboclistvalidator id=EmptyListValidator runat="server" errormessage="List is empty." enableclientscript="False" controltovalidate="EmptyList"></obrt:testboclistvalidator></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obw:boclist id=AllColumnsList runat="server" datasourcecontrol="EmptyDataSourceControl">
<fixedcolumns>
<obw:BocAllPropertiesPlacehoderColumnDefinition CssClass="test" Width="80%"></obw:BocAllPropertiesPlacehoderColumnDefinition>
<obw:BocEditDetailsColumnDefinition ItemID="EditDetails" SaveText="Save" CancelText="Cancel" EditText="Edit"></obw:BocEditDetailsColumnDefinition>
</FixedColumns>
</obw:boclist></td></tr>--%>
</table>
<p><asp:button id=ChildrenListEndEditModeButton runat="server" Text="End Edit Mode"></asp:button><asp:button id=ChildrenListAddAndEditButton runat="server" Text="Add and Edit"></asp:button></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><obr:reflectionbusinessobjectdatasourcecontrol id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/><obr:reflectionbusinessobjectdatasourcecontrol id=EmptyDataSourceControl runat="server" typename="OBRTest.Person, OBRTest"/></div>
