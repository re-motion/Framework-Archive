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
          <td><obc:boclist id=BocList runat="server" propertyidentifier="Children" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<fixedcolumns>
<obc:BocCommandColumnDefinition Label="Event">
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
<obc:BocMenuItem ItemID="" Icon="" Text="Event" Category="">
<persistedcommand>
<obc:BocCommand Type="Event"></obc:BocCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="" Text="Item" Category="">
<persistedcommand>
<obc:BocCommand Type="WxeFunction" WxeFunctionCommand-TypeName="MyType, MyAssembly"></obc:BocCommand>
</PersistedCommand>
</obc:BocMenuItem>
<obc:BocMenuItem ItemID="" Icon="" Text="Other Item" Category="">
<persistedcommand>
<obc:BocCommand Type="Href"></obc:BocCommand>
</PersistedCommand>
</obc:BocMenuItem>
</ListMenuItems>
            </obc:boclist></td></tr></table>
      <p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol id=ReflectionBusinessObjectDataSourceControl runat="server" TypeName="OBWTest.Person, OBWTest"></obr:reflectionbusinessobjectdatasourcecontrol></p>
      <p><asp:button id=Button1 runat="server" Text="Post Back"></asp:button></p></form>

  </body>
</html>
