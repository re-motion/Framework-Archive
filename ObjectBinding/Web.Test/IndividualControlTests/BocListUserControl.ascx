<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td></tr>
  <tr>
    <td>Jobs</td>
    <td><rubicon:boclist id=JobList runat="server" datasourcecontrol="CurrentObject" showallproperties="True" showavailableviewslist="False" showsortingorder="True" propertyidentifier="Jobs" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="2" selection="SingleRadioButton" index="Disabled">
</rubicon:boclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:TestBocList id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple">
<fixedcolumns>
<rubicon:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" Width="2em" EditText="Edit"></rubicon:BocRowEditModeColumnDefinition>
<rubicon:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event" CommandStateType="Rubicon.ObjectBinding.Sample::PersonListItemCommandState" ToolTip="An Event Command"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<rubicon:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<rubicon:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<rubicon:PropertyPathBinding PropertyPathIdentifier="LastName"></rubicon:PropertyPathBinding>
<rubicon:PropertyPathBinding PropertyPathIdentifier="FirstName"></rubicon:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCompoundColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="4em" ColumnTitle="Partner">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="Rubicon.ObjectBinding.Sample::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></rubicon:BocCustomColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></rubicon:BocDropDownMenuColumnDefinition>
</FixedColumns>
</ros:TestBocList></td></tr>
<%--  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:testboclist id=Testboclist1 runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" indexoffset="100" ShowEmptyListMessage="True" enableselection="True" Index="SortedOrder" Selection="Multiple" rowmenudisplay="Automatic" readonly="True">
<fixedcolumns>
<rubicon:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<rubicon:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<rubicon:PropertyPathBinding PropertyPathIdentifier="LastName"></rubicon:PropertyPathBinding>
<rubicon:PropertyPathBinding PropertyPathIdentifier="FirstName"></rubicon:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCompoundColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="4em" ColumnTitle="Partner">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
</FixedColumns>
</ros:testboclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:testboclist id=EmptyList runat="server" datasourcecontrol="EmptyDataSourceControl" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple" required="True" readonly="False">
<listmenuitems>
<rubicon:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
</ListMenuItems>

<optionsmenuitems>
<rubicon:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<rubicon:BocMenuItemCommand Type="Event"></rubicon:BocMenuItemCommand>
</PersistedCommand>
</rubicon:BocMenuItem>
</OptionsMenuItems>

<fixedcolumns>
<rubicon:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" EditText="Edit"></rubicon:BocRowEditModeColumnDefinition>
<rubicon:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event" CommandStateType="Rubicon.ObjectBinding.Sample::PersonListItemCommandState" ToolTip="An Event Command"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<rubicon:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<rubicon:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<rubicon:PropertyPathBinding PropertyPathIdentifier="LastName"></rubicon:PropertyPathBinding>
<rubicon:PropertyPathBinding PropertyPathIdentifier="FirstName"></rubicon:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCompoundColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" ColumnTitle="Partner">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<rubicon:BocListItemCommand Type="Event"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="Rubicon.ObjectBinding.Sample::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></rubicon:BocCustomColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></rubicon:BocDropDownMenuColumnDefinition>
</FixedColumns>
</ros:testboclist><ros:testboclistvalidator id=EmptyListValidator runat="server" errormessage="List is empty." enableclientscript="False" controltovalidate="EmptyList"></ros:testboclistvalidator></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><rubicon:boclist id=AllColumnsList runat="server" datasourcecontrol="EmptyDataSourceControl">
<fixedcolumns>
<rubicon:BocAllPropertiesPlacehoderColumnDefinition CssClass="test" Width="80%"></rubicon:BocAllPropertiesPlacehoderColumnDefinition>
<rubicon:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" EditText="Edit"></rubicon:BocRowEditModeColumnDefinition>
</FixedColumns>
</rubicon:boclist></td></tr>--%>
</table>
<p><asp:button id=ChildrenListEndEditModeButton runat="server" Text="End Edit Mode"></asp:button><asp:button id=ChildrenListAddAndEditButton runat="server" Text="Add and Edit"></asp:button></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/><rubicon:BindableObjectDataSourceControl id=EmptyDataSourceControl runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/></div>
