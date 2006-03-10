<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="OBWTest.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>

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

<rubicon:submenutab Text="BocList as Grid" ItemID="BocListAsGrid">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListAsGridUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocMultilineTextValue" ItemID="BocMultilineTextValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocMultilineTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="BocReferenceValue" ItemID="BocReferenceValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
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
</Tabs>
</rubicon:TabbedMenu>
<div style="WIDTH: 100%;TEXT-ALIGN: right">
WAI Conformance Level: 
<obw:BocEnumValue id="WaiConformanceLevelField" runat="server">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obw:BocEnumValue>
</div>
