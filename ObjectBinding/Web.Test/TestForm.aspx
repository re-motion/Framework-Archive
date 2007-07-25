<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>

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
<rwc:LazyContainer runat="server" id="LazyContainer"><obw:bocTextValue id="TextField" runat="server" readonly="True"></obw:bocTextValue></rwc:LazyContainer>
<p><asp:Button id="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
<table id="FormGrid" cellspacing="1" cellpadding="1" width="300" border="1" runat="server">
  <tr>
    <td colspan="2"></td></tr>
  <tr>
    <td></td>
    <td><asp:textbox id="TextBox1" runat="server"></asp:textbox><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="TextBox1" enableclientscript="False"></asp:requiredfieldvalidator></td></tr>
  <tr>
    <td></td>
    <td><obw:boctextvalue id="field" runat=server required="True" valuetype="String">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></obw:boctextvalue></td></tr>
</table>
<rwc:formgridmanager id="fgm" runat="server"/>
</form>

  </body>
</html>
