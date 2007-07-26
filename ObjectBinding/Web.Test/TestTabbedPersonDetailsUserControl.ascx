


<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonDetailsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonDetailsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<table id="FormGrid" runat="server" style="MARGIN-TOP: 0%">
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="LastNameField" required="true" runat="server" propertyidentifier="LastName" datasourcecontrol="CurrentObject">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="FirstNameField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocdatetimevalue id="DateOfBirthField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject" >
<datetimetextboxstyle autopostback="True">
</DateTimeTextBoxStyle></rubicon:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich" autopostback="True"></rubicon:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocdatetimevalue id="DateOfDeathField" runat="server" propertyidentifier="DateOfDeath" datasourcecontrol="CurrentObject" ></rubicon:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id="MarriageStatusField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocreferencevalue id="PartnerField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<rubicon:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocCommand>
</PersistedCommand></rubicon:bocreferencevalue></td></tr>
</table>
<p><rubicon:formgridmanager id="FormGridManager" runat="server" visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" typename="Rubicon.ObjectBinding.Sample::Person" /></p>
