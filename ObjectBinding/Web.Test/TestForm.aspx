<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Test Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<table id=FormGrid width="80%" runat="server">
  <tr>
    <td></td>
    <td><obc:boclist id=BocList runat="server" listmenulinebreaks="BetweenGroups" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Children" enableselection="True">
<fixedcolumns>
<obc:BocCommandColumnDefinition Text="Event">
<persistedcommand>
<obc:BocListItemCommand Type="Event"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocCommandColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
<persistedcommand>
<obc:BocListItemCommand></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
<obc:BocCompoundColumnDefinition FormatString="{0} {1}" ColumnTitle="Name">
<propertypathbindings>
<obc:PropertyPathBinding PropertyPathIdentifier="FirstName"></obc:PropertyPathBinding>
<obc:PropertyPathBinding PropertyPathIdentifier="LastName"></obc:PropertyPathBinding>
</PropertyPathBindings>

<persistedcommand>
<obc:BocListItemCommand></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocCompoundColumnDefinition>
</FixedColumns>

<optionsmenuitems>
<obc:BocMenuItem Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Wxe" IconDisabled="Images/RefelctionBusinessObjectIconDisabled.gif" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="Test'Test" WxeFunctionCommand-TypeName="MyType"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="Event">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Href">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Open" Text="&#214;ffnen" Category="Object" RequiredSelection="ExactlyOne">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Copy" Icon="Images/CopyItem.gif" Text="Kopieren" Category="Edit" IconDisabled="Images/CopyItemDisabled.gif" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Cut" Text="Ausschneiden" Category="Edit" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Paste" Text="Einf&#252;gen" Category="Edit">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Duplicate" Text="Duplizieren" Category="Edit" RequiredSelection="ExactlyOne">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="Delete" Icon="Images/DeleteItem.gif" Text="L&#246;schen" Category="Edit" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="javascript:DoSomething();"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems>

<listmenuitems>
<obc:BocMenuItem Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Event" Category="PostBacks">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Href" Category="Links">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Wxe" Category="PostBacks" IconDisabled="Images/RefelctionBusinessObjectIconDisabled.gif" RequiredSelection="OneOrMore">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-TypeName="MyType, MyAssembly"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="long text">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="long text">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="long text">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</ListMenuItems>
</obc:boclist></td></tr>
<tr><td></td><td><obc:BocList id="BocList1" runat="server" DataSourceControl="ReflectionBusinessObjectDataSourceControl" PropertyIdentifier="Jobs" ShowAllProperties="True">
<optionsmenuitems>
<obc:BocMenuItem Text="test">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem Text="test">
<persistedcommand>
<obc:BocMenuItemCommand></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems></obc:BocList></td></tr>
</table>
<p><rwc:formgridmanager id=FormGridManager runat="server" 
visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
TypeName="OBWTest.Person, OBWTest"></obr:reflectionbusinessobjectdatasourcecontrol></p>
<p><asp:button id=Button1 runat="server" Text="Post Back"></asp:button></p>
<p><asp:label id=EventLabel runat="server">###</asp:label></p></form>

  </body>
</html>
