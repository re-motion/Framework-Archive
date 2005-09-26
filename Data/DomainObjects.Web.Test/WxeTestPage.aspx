<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.WxeTestPage" %>
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
<TABLE id="Table1" style="WIDTH: 472px; HEIGHT: 127px" cellSpacing="1" cellPadding="1" 
width="472" border="1">
  <TR>
    <TD style="WIDTH: 191px">Transactions set correctly:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewButton" runat="server" Text="TransactionMode = CreateNew"></asp:Button></TD></TR>
  <TR>
    <TD style="WIDTH: 191px">Transactions set correctly:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneButton" runat="server" Text="TransactionMode = None"></asp:Button></TD></TR></TABLE><BR>
<TABLE id="Table2" style="WIDTH: 472px; HEIGHT: 85px" cellSpacing="1" cellPadding="1" 
width="472" border="1">
  <TR>
    <TD style="WIDTH: 190px">
      <P align=right>TransactionMode:</P></TD>
    <TD style="WIDTH: 141px">CreateNew</TD>
    <TD>None</TD></TR>
  <TR>
    <TD style="WIDTH: 190px">AutoCommit = true</TD>
    <TD style="WIDTH: 141px">
<asp:Button id="WxeTransactedFunctionCreateNewAutoCommitButton" runat="server" Text="Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneAutoCommitButton" runat="server" Text="Test"></asp:Button></TD></TR>
  <TR>
    <TD style="WIDTH: 190px">
      <P>AutoCommit = false</P></TD>
    <TD style="WIDTH: 141px">
<asp:Button id="WxeTransactedFunctionCreateNewNoAutoCommitButton" runat="server" Text="Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionNoneNoAutoCommitButton" runat="server" Text="Test"></asp:Button></TD></TR></TABLE></P></form>
	
  </body>
</HTML>
