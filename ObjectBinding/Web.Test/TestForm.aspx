<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Test Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<table id=FormGrid runat="server">
  <tr>
    <td></td>
    <td></td></tr></table>
<p><asp:Button id="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
<p><rwc:FormGridManager id="FormGridManager" runat="server"></rwc:FormGridManager><obw:BusinessObjectReferenceDataSourceControl id="BusinessObjectReferenceDataSourceControl1" runat="server"></obw:BusinessObjectReferenceDataSourceControl><obw:BocTextValue id="BocTextValue1" runat="server" DataSourceControl="BocTextValue1">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</obw:BocTextValue></p></form>

  </body>
</html>
