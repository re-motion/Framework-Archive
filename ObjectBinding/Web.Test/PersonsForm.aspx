<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="PersonsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonsForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Persons Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>Persons Form</h1>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=2>Persons</td></tr>
        <tr>
          <td></td>
          <td><obc:BocList id="PersonList" runat="server" PropertyIdentifier="" DataSourceControl="ReflectionBusinessObjectDataSourceControl" ShowAllProperties="True" >
<fixedcolumns>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
<persistedcommand>
<obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
</PersistedCommand>
</obc:BocSimpleColumnDefinition>
</FixedColumns></obc:BocList></td></tr>
          </table>
      <p><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBWTest.Person, OBWTest"></obr:reflectionbusinessobjectdatasourcecontrol></p></form>

  </body>
</html>
