<%@ Page Language="c#" CodeBehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.WebForm1" %>

<%@ Import Namespace="System" %>
<%@ Register TagPrefix="uc1" TagName="UserControl1" Src="UserControl1.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>WebForm1</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body ms_positioning="GridLayout">
    <form id="Form1" method="post" runat="server">
        <asp:TextBox ID="TextBox1" Style="Z-INDEX: 100; LEFT: 80px; POSITION: absolute; TOP: 48px" runat="server"></asp:TextBox>
        <asp:Button ID="ThrowText" Style="Z-INDEX: 118; LEFT: 128px; POSITION: absolute; TOP: 200px"
            runat="server" Text='Throw ("test")'></asp:Button>
        <asp:Label ID="Var2Label" Style="Z-INDEX: 110; LEFT: 376px; POSITION: absolute; TOP: 56px"
            runat="server" Height="32px" Width="488px"></asp:Label>
        <asp:Label ID="Label5" Style="Z-INDEX: 109; LEFT: 288px; POSITION: absolute; TOP: 56px" runat="server">Var2</asp:Label>
        <asp:Label ID="Var1Label" Style="Z-INDEX: 103; LEFT: 376px; POSITION: absolute; TOP: 24px"
            runat="server" Height="32px" Width="488px"></asp:Label>
        <asp:Button ID="Stay" Style="Z-INDEX: 101; LEFT: 40px; POSITION: absolute; TOP: 144px" runat="server"
            Text="Stay"></asp:Button>
        <asp:Button ID="Next" Style="Z-INDEX: 102; LEFT: 216px; POSITION: absolute; TOP: 144px" runat="server"
            Text="Next"></asp:Button>
        <div style="WIDTH: 16px; HEIGHT: 16.5em"></div>
        <asp:TextBox ID="SubNoReturnField" Style="Z-INDEX: 117; LEFT: 904px; POSITION: absolute; TOP: 72px"
            runat="server" Width="248px" AutoPostBack="True">change text for SubNoReturn</asp:TextBox>
        <asp:Button ID="SubNoReturnButton" Style="Z-INDEX: 116; LEFT: 904px; POSITION: absolute; TOP: 32px"
            runat="server" Text="SubNoReturn"></asp:Button>
        <asp:Button ID="SubExtButton" Style="Z-INDEX: 115; LEFT: 136px; POSITION: absolute; TOP: 144px"
            runat="server" Text="SubExt"></asp:Button>
        <asp:Label ID="RetValLabel" Style="Z-INDEX: 114; LEFT: 376px; POSITION: absolute; TOP: 88px"
            runat="server" Width="488px"></asp:Label>
        <asp:Label ID="Label1" Style="Z-INDEX: 113; LEFT: 288px; POSITION: absolute; TOP: 88px" runat="server">RetVal</asp:Label>
        <uc1:UserControl1 id="UserControl11" runat="server"></uc1:UserControl1>
        <asp:Button ID="Throw" Style="Z-INDEX: 112; LEFT: 56px; POSITION: absolute; TOP: 200px" runat="server"
            Text="Throw"></asp:Button>
        <asp:Label ID="Label3" Style="Z-INDEX: 108; LEFT: 280px; POSITION: absolute; TOP: 128px" runat="server">Stack</asp:Label>
        <asp:Label ID="Label2" Style="Z-INDEX: 107; LEFT: 288px; POSITION: absolute; TOP: 24px" runat="server">Var1</asp:Label>
        <asp:Label ID="StackLabel" Style="Z-INDEX: 106; LEFT: 376px; POSITION: absolute; TOP: 128px"
            runat="server" Height="168px" Width="480px"></asp:Label>
        <asp:Button ID="Sub" Style="Z-INDEX: 105; LEFT: 96px; POSITION: absolute; TOP: 144px" runat="server"
            Text="Sub"></asp:Button>
        <asp:CheckBox ID="IsPostBackCheck" Style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 96px"
            runat="server" Text="IsPostBack" Enabled="False"></asp:CheckBox>
    </form>
</body>
</html>
