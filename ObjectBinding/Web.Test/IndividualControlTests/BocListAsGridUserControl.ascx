<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListAsGridUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListAsGridUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><obw:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></obw:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obrt:testboclist id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="0" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple" errormessage="test" showeditmodevalidationmarkers="True">
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
<obw:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName" EnforceWidth="True" Width="5em">
<persistedcommand>
<obw:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocSimpleColumnDefinition>
<obw:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" EnforceWidth="True" Width="3em" ColumnTitle="Name">
<propertypathbindings>
<obw:PropertyPathBinding PropertyPathIdentifier="LastName"></obw:PropertyPathBinding>
<obw:PropertyPathBinding PropertyPathIdentifier="FirstName"></obw:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
</PersistedCommand>
</obw:BocCompoundColumnDefinition>
<obw:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" Width="7em" ColumnTitle="Partner">
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

</table>
<p><rubicon:WebButton id="SwitchToEditModeButton" runat="server" Text="Switch to Edit Mode"></rubicon:WebButton><rubicon:WebButton id="EndEditModeButton" runat="server" Text="End Edit Mode"></rubicon:WebButton><rubicon:WebButton id="CancelEditModeButton" runat="server" Text="Cancel Edit Mode"></rubicon:WebButton></p>
<p><rubicon:WebButton id="AddRowButton" runat="server" Text="Add Row"></rubicon:WebButton><obw:BocTextValue id="NumberOfNewRowsField" runat="server" ValueType="Integer" Width="2em" Required="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</obw:BocTextValue><rubicon:WebButton id="AddRowsButton" runat="server" Text="Add Rows"></rubicon:WebButton><rubicon:WebButton id="RemoveRows" runat="server" Text="Remove Rows"></rubicon:WebButton></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><obr:reflectionbusinessobjectdatasourcecontrol id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/><obr:reflectionbusinessobjectdatasourcecontrol id=EmptyDataSourceControl runat="server" typename="OBRTest.Person, OBRTest"/></div>
