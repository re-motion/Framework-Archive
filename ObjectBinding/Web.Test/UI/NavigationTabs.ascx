<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="OBWTest.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>

<rubicon:TabbedMenu id="TabbedMenu" runat="server">
<tabs>
<rubicon:MainMenuTab Text="Tests by Control" ItemID="IndividualControlTests">
<submenutabs>
<rubicon:SubMenuTab Text="BocBooleanValue" ItemID="BocBooleanValue">
<persistedcommand>
<rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocBooleanValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>

<rubicon:submenutab Text="BocCheckBox" ItemID="BocCheckBox">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocCheckBoxUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocDateTimeValue" ItemID="BocDateTimeValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocDateTimeValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocEnumValue" ItemID="BocEnumValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocEnumValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocList" ItemID="BocList">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocMultilineTextValue" ItemID="BocMultilineTextValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocMultilineTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocReferenceValue" ItemID="BocReferenceValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocReferenceValueControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocTextValue" ItemID="BocTextValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

</SubMenuTabs>

<persistedcommand>
<rubicon:NavigationCommand Type="None"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:MainMenuTab>
</Tabs></rubicon:TabbedMenu>
