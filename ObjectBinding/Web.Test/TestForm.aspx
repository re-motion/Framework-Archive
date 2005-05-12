<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
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
    <td><rwc:SmartLabel id="SmartLabel1" runat="server" forcontrol="BocTextValue1"></rwc:SmartLabel></td>
    <td>
      </td></tr>
  <tr>
    <td colspan="2"><obw:BocTextValue id="BocTextValue1" runat="server"></obw:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr></table>
<p><asp:Button id="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
<p><rwc:FormGridManager id="FormGridManager1" runat="server"></rwc:FormGridManager></p></form>

  </body>
</html>
