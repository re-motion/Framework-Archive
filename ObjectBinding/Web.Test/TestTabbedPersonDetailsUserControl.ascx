<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonDetailsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonDetailsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<table id="FormGrid" runat="server" style="MARGIN-TOP: 60%">
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="LastNameField" required="true" runat="server" propertyidentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="FirstNameField" runat="server" propertyidentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></rubicon:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocdatetimevalue id="DateOfBirthField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" requirederrormessage="Eingabe erforderlich">
<datetimetextboxstyle autopostback="True">
</DateTimeTextBoxStyle></rubicon:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocbooleanvalue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" nullitemerrormessage="Eingabe erforderlich" autopostback="True"></rubicon:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocdatetimevalue id="DateOfDeathField" runat="server" propertyidentifier="DateOfDeath" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" requirederrormessage="Eingabe erforderlich"></rubicon:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocenumvalue id="MarriageStatusField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:bocreferencevalue id="PartnerField" runat="server" propertyidentifier="Partner" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" nullitemerrormessage="Eingabe erforderlich">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<rubicon:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></rubicon:BocCommand>
</PersistedCommand></rubicon:bocreferencevalue></td></tr>
</table>
<p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></p>
