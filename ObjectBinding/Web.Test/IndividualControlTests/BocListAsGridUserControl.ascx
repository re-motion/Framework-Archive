<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocListAsGridUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocListAsGridUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><ros:TestBocList id=ChildrenList runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" alwaysshowpageinfo="True" listmenulinebreaks="BetweenGroups" pagesize="0" indexoffset="100" RowMenuDisplay="Manual" ShowEmptyListMessage="True" enableselection="True" Index="InitialOrder" Selection="Multiple" errormessage="test" showeditmodevalidationmarkers="True">
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
<rubicon:BocSimpleColumnDefinition ItemID="LastName" PropertyPathIdentifier="LastName" EnforceWidth="True" Width="5em">
<persistedcommand>
<rubicon:BocListItemCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocCompoundColumnDefinition ItemID="Name" FormatString="{0}, {1}" EnforceWidth="True" Width="3em" ColumnTitle="Name">
<propertypathbindings>
<rubicon:PropertyPathBinding PropertyPathIdentifier="LastName"></rubicon:PropertyPathBinding>
<rubicon:PropertyPathBinding PropertyPathIdentifier="FirstName"></rubicon:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCompoundColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="Partner" Width="7em" ColumnTitle="Partner">
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

</table>
<p><rubicon:WebButton id="SwitchToEditModeButton" runat="server" Text="Switch to Edit Mode"></rubicon:WebButton><rubicon:WebButton id="EndEditModeButton" runat="server" Text="End Edit Mode"></rubicon:WebButton><rubicon:WebButton id="CancelEditModeButton" runat="server" Text="Cancel Edit Mode"></rubicon:WebButton></p>
<p><rubicon:WebButton id="AddRowButton" runat="server" Text="Add Row"></rubicon:WebButton><rubicon:BocTextValue id="NumberOfNewRowsField" runat="server" ValueType="Integer" Width="2em" Required="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</rubicon:BocTextValue><rubicon:WebButton id="AddRowsButton" runat="server" Text="Add Rows"></rubicon:WebButton><rubicon:WebButton id="RemoveRows" runat="server" Text="Remove Rows"></rubicon:WebButton></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/><rubicon:BindableObjectDataSourceControl id=EmptyDataSourceControl runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/></div>
