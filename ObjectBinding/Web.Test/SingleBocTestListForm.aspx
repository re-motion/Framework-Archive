<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="SingleBocTestListForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocListForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: List Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>SingleBocTest: List Form</h1>
<table id=FormGrid width="80%" runat="server">
  <tr>
    <td colSpan=2><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:boctextvalue>&nbsp;<obc:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" ReadOnly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:boctextvalue></td></tr>
  <tr>
    <td>Jobs</td>
    <td><obc:boclist id=JobList runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" showsortingorder="True" showadditionalcolumnslist="False" propertyidentifier="Jobs" alwaysshowpageinfo="True" showadditionalcolumnsselection="False" listmenulinebreaks="BetweenGroups">
<fixedcolumns>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="Title" ColumnID="Title">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<obc:BocListItemCommand Type="Event"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
</FixedColumns>

<optionsmenuitems>
<obc:BocMenuItem Text="Wxe" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="Test'Test" WxeFunctionCommand-TypeName="MyType"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Event">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Href" Style="Text">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems>

<listmenuitems>
<obc:BocMenuItem Text="Event" Category="PostBacks">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Href" Category="Links" Style="Text">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Wxe" Category="PostBacks" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-TypeName="MyType, MyAssembly"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</ListMenuItems>
            </obc:boclist></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><obc:boclist id=ChildrenList runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Children" alwaysshowpageinfo="True" showadditionalcolumnsselection="False" pagesize="2" enableselection="True" ShowSortingOrder="True" listmenulinebreaks="BetweenGroups">
<fixedcolumns>
<obc:BocCommandColumnDefinition Text="Edit" ColumnID="Edit" ColumnTitle="Cmd">
<persistedcommand>
<obc:BocListItemCommand Type="Href" HrefCommand-Href="edit.aspx?ID={1}&amp;Index={0}"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocCommandColumnDefinition>
<obc:BocCommandColumnDefinition Text="Event" ColumnID="E1">
<persistedcommand>
<obc:BocListItemCommand Type="Event"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocCommandColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="LastName" ColumnID="LastName">
<persistedcommand>
<obc:BocListItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocCompoundColumnDefinition FormatString="{0}, {1}" ColumnID="Name" ColumnTitle="Name">
<propertypathbindings>
<obc:PropertyPathBinding PropertyPathIdentifier="LastName"></obc:PropertyPathBinding>
<obc:PropertyPathBinding PropertyPathIdentifier="FirstName"></obc:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocCompoundColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="Partner.FirstName" ColumnTitle="Partner">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="LastName">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocCustomColumnDefinition PropertyPathIdentifier="LastName" CustomCellType="OBRTest::PersonCustomCell" ColumnID="CustomCell" ColumnTitle="Custom Cell"></obc:BocCustomColumnDefinition>
</FixedColumns>

<optionsmenuitems>
<obc:BocMenuItem Text="Open" ItemID="Open" Category="Object" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="objects" WxeFunctionCommand-TypeName="OBWTest.ViewPersonsWxeFunction,OBWTest"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Copy" ItemID="Copy" Icon="Images/CopyItem.gif" Category="Edit" RequiredSelection="OneOrMore" DisabledIcon="Images/CopyItemDisabled.gif">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Cut" ItemID="Cut" Category="Edit" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Paste" ItemID="Paste" Category="Edit">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Duplicate" ItemID="Duplicate" Category="Edit">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Delete" ItemID="Delete" Icon="Images/DeleteItem.gif" Category="Edit" RequiredSelection="OneOrMore" Style="Icon">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems>

<listmenuitems>
<obc:BocMenuItem Text="Paste" ItemID="Paste" Category="Edit" IsDisabled="True">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Delete" ItemID="Delete" Icon="Images/DeleteItem.gif" Category="Edit" RequiredSelection="OneOrMore" DisabledIcon="Images/DeleteItemDisabled.gif" Style="Icon">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</ListMenuItems>

            </obc:boclist></td></tr></table>
<p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p>
<p><asp:checkbox id=ChildrenListEventCheckBox runat="server" Text="ChildrenList Event raised" enableviewstate="False" Enabled="False"></asp:checkbox></p>
<p></p>
<p><asp:label id=ChildrenListEventArgsLabel runat="server" enableviewstate="False"></asp:label></p>
<div runat="server" visible="false"><rwc:formgridmanager 
id=FormGridManager 
runat="server"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></div></form>
  </body>
</html>
