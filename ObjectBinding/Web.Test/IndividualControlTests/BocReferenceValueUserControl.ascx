<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocReferenceValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocReferenceValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><obr:reflectionbusinessobjectdatasourcecontrol id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obw:boctextvalue id=FirstNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName">
</obw:boctextvalue>&nbsp;<obw:boctextvalue id=LastNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></obw:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=PartnerField runat="server" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" select="*" width="100%">
<persistedcommand>
<obw:BocCommand Type="Event"></obw:BocCommand>
</PersistedCommand>

<optionsmenuitems>
<obw:BocMenuItem Text="intern">
<persistedcommand>
<obw:BocMenuItemCommand Type="Href" HrefCommand-Href="~/startForm.aspx"></obw:BocMenuItemCommand>
</PersistedCommand>
</obw:BocMenuItem>
<obw:BocMenuItem Text="extern">
<persistedcommand>
<obw:BocMenuItemCommand Type="Href" HrefCommand-Target="_blank" HrefCommand-Href="~/startForm.aspx"></obw:BocMenuItemCommand>
</PersistedCommand>
</obw:BocMenuItem>
</OptionsMenuItems>

<labelstyle cssclass="class">
</LabelStyle></obw:bocreferencevalue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id=PartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=ReadOnlyPartnerField runat="server" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" width="100%" >
<persistedcommand>
<obw:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="Event"></obw:BocCommand>
</PersistedCommand>

<labelstyle cssclass="class">
</LabelStyle></obw:bocreferencevalue></td>
    <td>bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=UnboundPartnerField runat="server" required="True" hasvalueembeddedinsideoptionsmenu="False" width="250px" showoptionsmenu="False">
<persistedcommand>
<obw:boccommand Type="Event"></obw:boccommand>
</PersistedCommand></obw:bocreferencevalue></td>
    <td>
      <p> unbound, value not set</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=UnboundReadOnlyPartnerField runat="server" readonly="True" enableicon="False" hasvalueembeddedinsideoptionsmenu="False" width="250px">
<persistedcommand>
<obw:boccommand Type="Event"></obw:boccommand>
</PersistedCommand>

</obw:bocreferencevalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=DisabledPartnerField runat="server" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" embeddedvalue="False" hasvalueembeddedinsideoptionsmenu="True" enabled="False">

<persistedcommand>
<obw:boccommand Type="Event"></obw:boccommand>
</PersistedCommand></obw:bocreferencevalue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=DisabledReadOnlyPartnerField runat="server" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" enabled="False" embeddedvalue="False">

<persistedcommand>
<obw:boccommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></obw:boccommand>
</PersistedCommand></obw:bocreferencevalue></td>
    <td>disabled, bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=DisabledUnboundPartnerField runat="server" required="True" enabled="False" embeddedvalue="False" width="250px">
<persistedcommand>
<obw:boccommand Type="None"></obw:boccommand>
</PersistedCommand></obw:bocreferencevalue></td>
    <td>
      <p> disabled, unbound, value set</p></td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><obw:bocreferencevalue id=DisabledUnboundReadOnlyPartnerField runat="server" readonly="True" enableicon="False" enabled="False" embeddedvalue="False" width="250px" >
<persistedcommand>
<obw:boccommand Type="None"></obw:boccommand>
</PersistedCommand>

</obw:bocreferencevalue></td>
    <td>
      <p>disabled, unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr></table>
<p>Partner Command Click: <asp:label id="PartnerCommandClickLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Selection Changed: <asp:label id=PartnerFieldSelectionChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Menu Click: <asp:label id=PartnerFieldMenuClickEventArgsLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><br><rubicon:webbutton id=PartnerTestSetNullButton runat="server" width="220px" Text="Partner Set Null"/><rubicon:webbutton id=PartnerTestSetNewItemButton runat="server" width="220px" Text="Partner Set New Item"/></p>
<p><rubicon:webbutton id=ReadOnlyPartnerTestSetNullButton runat="server" width="220px" Text="Read Only Partner Set Null"/><rubicon:webbutton id=ReadOnlyPartnerTestSetNewItemButton runat="server" width="220px" Text="Read Only Partner Set New Item"/></p>
