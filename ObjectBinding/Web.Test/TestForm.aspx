<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
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
    <td><obc:boclist id=BocList runat="server" propertyidentifier="Children" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<optionsmenuitems>
<obc:BocMenuItem ItemID="" Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Wxe" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-Parameters="Test'Test" WxeFunctionCommand-TypeName="MyType"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="" Text="Event" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Href" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:bocmenuitem ItemID="Open" Text="&#214;ffnen" Category="Object">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
<obc:bocmenuitem ItemID="Copy" Icon="Images/CopyItem.gif" Text="Kopieren" Category="Edit">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
<obc:bocmenuitem ItemID="Cut" Text="Ausschneiden" Category="Edit">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
<obc:bocmenuitem ItemID="Paste" Text="Einf&#252;gen" Category="Edit">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
<obc:bocmenuitem ItemID="Duplicate" Text="Duplizieren" Category="Edit">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
<obc:bocmenuitem ItemID="Delete" Icon="Images/DeleteItem.gif" Text="L&#246;schen" Category="Edit">
<persistedcommand>
<obc:bocmenuitemcommand></obc:bocmenuitemcommand>
</PersistedCommand>
</obc:bocmenuitem>
</OptionsMenuItems>

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

<listmenuitems>
<obc:BocMenuItem ItemID="" Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Event" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Wxe" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="WxeFunction" WxeFunctionCommand-TypeName="MyType, MyAssembly"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="Images/RefelctionBusinessObjectIcon.gif" Text="Href" Category="">
<persistedcommand>
<obc:BocMenuItemCommand Type="Href" HrefCommand-Href="link.htm"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</ListMenuItems>
</obc:boclist></td></tr></table>
<p><rwc:formgridmanager id=FormGridManager runat="server" 
visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
TypeName="OBWTest.Person, OBWTest"></obr:reflectionbusinessobjectdatasourcecontrol></p>
<p><asp:button id=Button1 runat="server" Text="Post Back"></asp:button></p>
<p><asp:label id=EventLabel runat="server">###</asp:label></p></form>

  </body>
</html>
