



<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocTextValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocTextValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" typename="Rubicon.ObjectBinding.Sample::Person"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4>Person</td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=FirstNameField runat="server" Width="150px" PropertyIdentifier="FirstName" required="True" datasourcecontrol="CurrentObject">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td>
      <p>bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=FirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=ReadOnlyFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" ReadOnly="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=UnboundFirstNameField runat="server" Width="150px">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td>
      <p>unbound, value not set, list-box, 
      required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=UnboundReadOnlyFirstNameField runat="server" Width="150px" ReadOnly="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=IncomeField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="Income" ReadOnly="True" format="c">
              <textboxstyle textmode="SingleLine">
              </textboxstyle></rubicon:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label1 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=HeightField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="Height">
<textboxstyle textmode="SingleLine" maxlength="3">
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label4 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DateOfBirthField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="DateOfBirth">
              <textboxstyle textmode="SingleLine">
              </textboxstyle></rubicon:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label2 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DateOfDeathField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="DateOfDeath">
              <textboxstyle textmode="SingleLine">
              </textboxstyle></rubicon:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label3 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DisabledFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" required="True" enabled="false">
              <textboxstyle textmode="SingleLine">
              </textboxstyle>
            </rubicon:boctextvalue></td>
    <td>
      <p>disabled, bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DisabledReadOnlyFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" ReadOnly="True" enabled="false"></rubicon:boctextvalue></td>
    <td>
      <p>disabled, bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DisabledUnboundFirstNameField runat="server" Width="150px" enabled="false"></rubicon:boctextvalue></td>
    <td>
      <p>disabled, unbound, value set, list-box, 
      required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id=DisabledUnboundReadOnlyFirstNameField runat="server" Width="150px" ReadOnly="True" enabled="false"></rubicon:boctextvalue></td>
    <td>
      <p>disabled, unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="BocTextValue1" runat="server" ValueType="Integer" required="True" numberstyle="AllowThousands">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td></td>
    <td style="WIDTH: 20%"></td></tr>
  <tr>
    <td></td>
    <td><rubicon:boctextvalue id="BocTextValue2" runat="server" ValueType="Double">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td></td>
    <td style="WIDTH: 20%"></td></tr></table>
<p><rubicon:webbutton id=FirstNameTestSetNullButton runat="server" Text="FirstName Set Null" width="220px"/><rubicon:webbutton id=FirstNameTestSetNewValueButton runat="server" Text="FirstName Set New Value" width="220px"/></p>
<p>FirstName Field Text Changed: <asp:label id=FirstNameFieldTextChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><br><rubicon:webbutton id=ReadOnlyFirstNameTestSetNullButton runat="server" Text="Read Only FirstName Set Null" width="220px"/><rubicon:webbutton id=ReadOnlyFirstNameTestSetNewValueButton runat="server" Text="Read Only FirstName Set New Value" width="220px"/></p>
