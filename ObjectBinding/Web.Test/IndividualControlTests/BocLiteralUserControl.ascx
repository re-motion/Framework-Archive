<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BocLiteralUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocLiteralUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/></div>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=4><rubicon:boctextvalue id=FirstNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName"></rubicon:boctextvalue>&nbsp;<rubicon:boctextvalue id=LastNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></rubicon:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:BocLiteral id=CVField runat="server" datasourcecontrol="CurrentObject" PropertyIdentifier="CVString" /></td>
          <td>
            <p>bound</p></td>
          <td style="WIDTH: 20%"><asp:label id=CVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><rubicon:BocLiteral id=UnboundCVField runat="server" /></td>
          <td>
            <p>unbound, value not set</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
      </table>
      <p><rubicon:webbutton id=CVTestSetNullButton runat="server" Text="VC Set Null" width="220px"/><rubicon:webbutton id=CVTestSetNewValueButton runat="server" Text="CVSet New Value" width="220px"/></p>
