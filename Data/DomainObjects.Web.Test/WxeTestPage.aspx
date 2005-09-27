<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.WxeTestPage" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>FirstPage</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </HEAD>
<body>
<form id=Form1 method=post 
runat="server">
<P>
<asp:Label id="ResultLabel" runat="server">ResultLabel</asp:Label></P>
<P>
<h2>WXE-TransactionMode</h2>
<TABLE id="Table1" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>TransactionMode = CreateNew:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewButton" runat="server" Text="Run Test"></asp:Button></TD></TR>
  <TR>
    <TD>TransactionMode = None:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneButton" runat="server" Text="Run Test"></asp:Button></TD></TR></TABLE><BR>

<h2>Write and read data with different values for TransactionMode and AutoCommit</h2>
<TABLE id="Table2" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>
      <P align=right>TransactionMode:</P></TD>
    <TD>CreateNew</TD>
    <TD>None</TD></TR>
  <TR>
    <TD>AutoCommit = true</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD></TR>
  <TR>
    <TD>
      <P>AutoCommit = false</P></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewNoAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneNoAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD></TR></TABLE>
<P></P></form>
	
  </body>
</HTML>
