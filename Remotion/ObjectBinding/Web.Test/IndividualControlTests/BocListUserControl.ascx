<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
  <tr>
    <td>Jobs</td>
    <td><remotion:boclist id=JobList runat="server" datasourcecontrol="CurrentObject" showallproperties="True" showavailableviewslist="False" propertyidentifier="Jobs" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" selection="SingleRadioButton" index="Disabled" PageSize="2" ShowSortingOrder="True">
</remotion:boclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:TestBocList id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="ChildrenAsObjects" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple">
<fixedcolumns>
<remotion:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" Width="2em" EditText="Edit"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<remotion:BocListItemCommand ToolTip="An Event Command" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState" Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<remotion:BocListItemCommand HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}" Type="Href"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName" IsDynamic="True">
<persistedcommand>
<remotion:BocListItemCommand WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" WxeFunctionCommand-Parameters="id" Type="WxeFunction"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">

<persistedcommand>
<remotion:BocListItemCommand></remotion:BocListItemCommand>
</PersistedCommand>
<propertypathbindings>
<remotion:PropertyPathBinding PropertyPathIdentifier="LastName" IsDynamic="True"></remotion:PropertyPathBinding>
<remotion:PropertyPathBinding PropertyPathIdentifier="FirstName" IsDynamic="True"></remotion:PropertyPathBinding>
</PropertyPathBindings>
</remotion:BocCompoundColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="6em" ColumnTitle="Partner" IsDynamic="True" EnableIcon="True" >
<persistedcommand>
<remotion:BocListItemCommand></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName" ColumnTitle="Parter" IsDynamic="True">
<persistedcommand>
<remotion:BocListItemCommand Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" ColumnTitle="LastName" IsReadOnly="True" IsDynamic="True">
<persistedcommand>
<remotion:BocListItemCommand></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="Remotion.ObjectBinding.Sample::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell" IsDynamic="True"></remotion:BocCustomColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased" IsSortable="False" ColumnTitle="Deceased" IsDynamic="True">
<persistedcommand>
<remotion:BocListItemCommand></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></remotion:BocDropDownMenuColumnDefinition>
</FixedColumns>
</ros:TestBocList></td></tr>
<%--  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:testboclist id=Testboclist1 runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" indexoffset="100" ShowEmptyListMessage="True" enableselection="True" Index="SortedOrder" Selection="Multiple" rowmenudisplay="Automatic" readonly="True">
<fixedcolumns>
<remotion:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<remotion:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<remotion:PropertyPathBinding PropertyPathIdentifier="LastName"></remotion:PropertyPathBinding>
<remotion:PropertyPathBinding PropertyPathIdentifier="FirstName"></remotion:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCompoundColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" EnforceWidth="True" Width="4em" ColumnTitle="Partner">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<remotion:BocListItemCommand Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns>
</ros:testboclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:testboclist id=EmptyList runat="server" datasourcecontrol="EmptyDataSourceControl" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="4" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple" required="True" readonly="False">
<listmenuitems>
<remotion:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
</PersistedCommand>
</remotion:BocMenuItem>
</ListMenuItems>

<optionsmenuitems>
<remotion:BocMenuItem Text="test" ItemID="test">
<persistedcommand>
<remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
</PersistedCommand>
</remotion:BocMenuItem>
</OptionsMenuItems>

<fixedcolumns>
<remotion:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" EditText="Edit"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="E1" Text="E 1" ColumnTitle="Cmd">
<persistedcommand>
<remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState" ToolTip="An Event Command"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition ItemID="Href" Text="Href">
<persistedcommand>
<remotion:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName">
<persistedcommand>
<remotion:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" ColumnTitle="Name">
<propertypathbindings>
<remotion:PropertyPathBinding PropertyPathIdentifier="LastName"></remotion:PropertyPathBinding>
<remotion:PropertyPathBinding PropertyPathIdentifier="FirstName"></remotion:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCompoundColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" ColumnTitle="Partner">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition ItemID="PartnerFirstName" PropertyPathIdentifier="Partner.FirstName">
<persistedcommand>
<remotion:BocListItemCommand Type="Event"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" IsReadOnly="True">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocCustomColumnDefinition ItemID="CustomCell" PropertyPathIdentifier="LastName" CustomCellType="Remotion.ObjectBinding.Sample::PersonCustomCell" Mode="ControlInEditedRow" ColumnTitle="Custom Cell"></remotion:BocCustomColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Deceased">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="0%" ColumnTitle="Menu"></remotion:BocDropDownMenuColumnDefinition>
</FixedColumns>
</ros:testboclist><ros:testboclistvalidator id=EmptyListValidator runat="server" errormessage="List is empty." enableclientscript="False" controltovalidate="EmptyList"></ros:testboclistvalidator></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><remotion:boclist id=AllColumnsList runat="server" datasourcecontrol="EmptyDataSourceControl">
<fixedcolumns>
<remotion:BocAllPropertiesPlacehoderColumnDefinition CssClass="test" Width="80%"></remotion:BocAllPropertiesPlacehoderColumnDefinition>
<remotion:BocRowEditModeColumnDefinition ItemID="EditRow" SaveText="Save" CancelText="Cancel" EditText="Edit"></remotion:BocRowEditModeColumnDefinition>
</FixedColumns>
</remotion:boclist></td></tr>--%>
</table>
<p><asp:button id=ChildrenListEndEditModeButton runat="server" Text="End Edit Mode"></asp:button><asp:button id=ChildrenListAddAndEditButton runat="server" Text="Add and Edit"></asp:button></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/><remotion:BindableObjectDataSourceControl id=EmptyDataSourceControl runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
