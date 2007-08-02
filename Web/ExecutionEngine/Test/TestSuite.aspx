<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSuite.aspx.cs" Inherits="Rubicon.Web.Test.TestSuite" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head>
    <title>Rubicon.Web.Tests</title>
    <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="Form" runat="server">
    <asp:Table ID="TestSuiteTable" runat="server">
      <asp:TableHeaderRow>
        <asp:TableHeaderCell>Test Suite</asp:TableHeaderCell>
      </asp:TableHeaderRow>
    </asp:Table>
  </form>
</body>
</html>
