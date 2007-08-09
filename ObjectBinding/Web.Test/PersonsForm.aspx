

<%@ Page language="c#" Codebehind="PersonsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonsForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Persons Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <rubicon:htmlheadcontents runat="server" id="HtmlHeadContents"></rubicon:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>Persons Form</h1>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=2>Persons</td></tr>
        <tr>
          <td></td>
          <td><rubicon:BocList id="PersonList" runat="server" PropertyIdentifier="" DataSourceControl="CurrentObject" ShowAllProperties="True" >
<fixedcolumns>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
</FixedColumns></rubicon:BocList></td></tr>
          </table>
      <p><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><rubicon:formgridmanager id=FormGridManager runat="server" visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Rubicon.ObjectBinding.Sample::Person" /></p></form>

  </body>
</html>
