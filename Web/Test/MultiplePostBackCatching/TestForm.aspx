<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestForm.aspx.cs" Inherits="Rubicon.Web.Test.MultiplePostBackCatching.TestForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body style="overflow: visible;">
  <form id="Form" runat="server">
    <asp:Table ID="TestTable" runat="server" EnableViewState="false" />
  </form>
</body>
</html>
