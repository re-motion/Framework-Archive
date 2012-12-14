<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="SingleBocTestReferenceValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocReferenceValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: ReferenceValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: ReferenceValue Form</h1>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
</obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True"></obc:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><obc:BocReferenceValue id="PartnerField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner" width="200px" readonly="False" nullitemerrormessage="Eingabe erforderlich">
<optionsmenuitems>
<obc:BocMenuItem Text="&#214;ffnen" ItemID="Open" Category="Object" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="objects" WxeFunctionCommand-TypeName="OBWTest.ViewPersonsWxeFunction,OBWTest"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Copy" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems></obc:BocReferenceValue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:Label id="PartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:Label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:BocReferenceValue id="ReadOnlyPartnerField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner" width="200px" readonly="True" nullitemerrormessage="Eingabe erforderlich">
<optionsmenuitems>
<obc:BocMenuItem Text="&#214;ffnen" ItemID="Open" Category="Object" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="objects" WxeFunctionCommand-TypeName="OBWTest.ViewPersonsWxeFunction,OBWTest"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Copy" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems></obc:BocReferenceValue></td>
    <td>bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:BocReferenceValue id="UnboundPartnerField" runat="server" width="200px" required="True" nullitemerrormessage="Eingabe erforderlich"></obc:BocReferenceValue></td>
    <td>
      <p>unbound, value not set, list-box</p></td>
    <td style="WIDTH: 20%"><asp:label id="UnboundPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
  </tr>
  <tr>
    <td></td>
    <td><obc:BocReferenceValue id="UnboundReadOnlyPartnerField" runat="server" readonly="True" width="200px" enableicon="False" nullitemerrormessage="Eingabe erforderlich">
<optionsmenuitems>
<obc:BocMenuItem Text="&#214;ffnen" ItemID="Open" Category="Object" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="objects" WxeFunctionCommand-TypeName="OBWTest.ViewPersonsWxeFunction,OBWTest"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Copy" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems></obc:BocReferenceValue></td>
    <td>
      <p>unbound, value set, read only</p></td> 
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
    </tr>
      </table>
<p>
      
      <asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
<p>Partner Selection Changed: <asp:Label id="PartnerFieldSelectionChangedLabel" runat="server" enableviewstate="False">#</asp:Label></p>
<p>Partner Menu Click: <asp:label id="PartnerFieldMenuClickEventArgsLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p>
      <br>
      <asp:button id=PartnerTestSetNullButton runat="server" Text="Partner Set Null" width="220px"></asp:button><asp:button id="PartnerTestSetNewItemButton" runat="server" Text="Partner Set New Item" width="220px"></asp:button></p>
<p>
      <asp:button id="ReadOnlyPartnerTestSetNullButton" runat="server" Text="Read Only Partner Set Null" width="220px"></asp:button><asp:button id="ReadOnlyPartnerTestSetNewItemButton" runat="server" Text="Read Only Partner Set New Item" width="220px"></asp:button></p>
<p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
<p><rwc:ValidationStateViewer id="ValidationStateViewer1" runat="server" visible="true" ></rwc:ValidationStateViewer></p></form>
	
  </body>
</html>
