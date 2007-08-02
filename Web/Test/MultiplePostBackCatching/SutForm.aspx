<%@ Page language="c#" Codebehind="SutForm.aspx.cs" AutoEventWireup="True" Inherits="Rubicon.Web.Test.MultiplePostBackCatching.SutForm" smartNavigation="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head id="Head" runat="server">
    <title>MultiplePostbackCatcherForm</title>
    <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
<script language=javascript>
  function ChangeAutoPostbackListSelection()
  {
    document.Form.AutoPostbackList.fireEvent ('onChange','');
  } 
</script>
<style type="text/css">
  TD.first { BACKGROUND-COLOR: lightgrey }
  </style>
</head>
<body>
<form id=Form method=post runat="server">
Test-Result: <asp:Label ID="TestResultLabel" runat="server" Text="###" EnableViewState="false" />
<asp:Table ID="TestMatrix" runat="server"></asp:Table>
<table style="WIDTH: 100%; HEIGHT: 100%">
  <tr>
    <td style="VERTICAL-ALIGN: top">
<a href="mpc.wxe?Parameter=Garbage;">Hyperlink</a><br>
<input type="text" onkeyup="ChangeAutoPostbackListSelection(); return false;" />
<input type="button" value="Select value" onclick="ChangeAutoPostbackListSelection();  return false;" />
      <asp:DropDownList ID="AutoPostbackList" runat="server" AutoPostBack="True" EnableViewState="false">
        <asp:ListItem Value="1" Text="1" Selected="True" />
        <asp:ListItem Value="2" Text="2" />
        <asp:ListItem Value="3" Text="3" />
      </asp:DropDownList></td></tr></table>
</form>
  </body>
</html>