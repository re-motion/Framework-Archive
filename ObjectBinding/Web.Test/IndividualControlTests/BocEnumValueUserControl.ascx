



<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocEnumValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocEnumValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" typename="Rubicon.ObjectBinding.Sample::Person"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" readonly="True"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=GenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
</rubicon:bocenumvalue></td>
    <td>
      <p>bound, radio buttons, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=GenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=ReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" width="150px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
</rubicon:bocenumvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=MarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False">
<listcontrolstyle radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p>bound, drop-down, required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=MarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=UnboundMarriageStatusField runat="server" >
<listcontrolstyle listboxrows="2" controltype="ListBox" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p>unbound, value not set, list-box, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=UnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True">
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" controltype="ListBox"
                radiobuttonlistcellspacing="" radiobuttonlistcellpadding="" radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DeceasedAsEnumField runat="server" PropertyIdentifier="Deceased" datasourcecontrol="CurrentObject" required="False">
              <listcontrolstyle radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding=""></ListControlStyle>
            </rubicon:bocenumvalue></td>
    <td>deceased (bool) as enum</td>
    <td style="WIDTH: 20%"><asp:label id=DeceasedAsEnumFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DisabledGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
</rubicon:bocenumvalue></td>
    <td>
      <p>disabled, bound, radio buttons, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DisabledReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
</rubicon:bocenumvalue></td>
    <td>
      <p>disabled, bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DisabledMarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False" enabled=false>
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" radiobuttonlistrepeatdirection="Horizontal"
                radiobuttonlistcellspacing="" radiobuttonlistcellpadding="" radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p>disabled, bound, drop-down, required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DisabledUnboundMarriageStatusField runat="server" enabled=false>
              <listcontrolstyle radiobuttonlisttextalign="Right" listboxrows="2" radionbuttonlistrepeatlayout="Table"
                controltype="ListBox" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p> disabled, unbound, value&nbsp;set, list-box, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id=DisabledUnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True" enabled=false>
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" controltype="ListBox"
                radiobuttonlistcellspacing="" radiobuttonlistcellpadding="" radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </rubicon:bocenumvalue></td>
    <td>
      <p>disabled, unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>Instance Enum</td>
    <td><rubicon:bocenumvalue id="InstanceEnumField" runat="server">
            </rubicon:bocenumvalue></td>
    <td>
      <p> unboud</p></td>
    <td style="WIDTH: 20%"><asp:label id="InstanceEnumFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
    </table>
<p><br>Gender Selection Changed: <asp:label id=GenderFieldSelectionChangedLabel runat="server" EnableViewState="False">#</asp:label></p>
<p><rubicon:webbutton id=GenderTestSetNullButton runat="server" Text="Gender Set Null" width="165px"/><rubicon:webbutton id=GenderTestSetDisabledGenderButton runat="server" Text="Gender Set Disabled Gender" width="165px"/><rubicon:webbutton id=GenderTestSetMarriedButton runat="server" Text="Gender Set Married" width="165px"/></p>
<p><br><rubicon:webbutton id=ReadOnlyGenderTestSetNullButton runat="server" Text="Read Only Gender Set Null" width="220px"/><rubicon:webbutton id=ReadOnlyGenderTestSetNewItemButton runat="server" Text="Read Only Gender Set Female" width="220px"/></p>
