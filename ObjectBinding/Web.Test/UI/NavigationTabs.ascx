
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="OBWTest.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<rubicon:TabbedMenu id="TabbedMenu" runat="server">
<tabs>
<rubicon:MainMenuTab Text="Tests by Control" ItemID="IndividualControlTests">
<submenutabs>
<rubicon:SubMenuTab Text="Boolean" ItemID="BocBooleanValue">
<persistedcommand>
<rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocBooleanValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:NavigationCommand>
</PersistedCommand>
</rubicon:SubMenuTab>

<rubicon:submenutab Text="CheckBox" ItemID="BocCheckBox">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocCheckBoxUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="DateTime" ItemID="BocDateTimeValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocDateTimeValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="Enum" ItemID="BocEnumValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocEnumValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="List" ItemID="BocList">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="List as Grid" ItemID="BocListAsGrid">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListAsGridUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="Literal" ItemID="BocLiteral">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocLiteralUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="MultilineText" ItemID="BocMultilineTextValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocMultilineTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="Reference" ItemID="BocReferenceValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="Auto Complete Reference" ItemID="BocAutoCompleteReferenceValue">
<persistedcommand>
<rubicon:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocAutoCompleteReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></rubicon:navigationcommand>
</PersistedCommand>
</rubicon:submenutab>

<rubicon:submenutab Text="Text" ItemID="BocTextValue">
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
<rubicon:BocEnumValue id="WaiConformanceLevelField" runat="server">
<listcontrolstyle autopostback="True" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:BocEnumValue>
</div>
