<%@ Register TagPrefix="uc1" TagName="UserControl1" Src="UserControl1.ascx" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>WebForm1</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
  </HEAD>
  <body MS_POSITIONING="GridLayout">
    <FORM id="Form" method="post" runat="server">
      <asp:textbox id="TextBox1" style="Z-INDEX: 101; LEFT: 80px; POSITION: absolute; TOP: 48px" runat="server"></asp:textbox><asp:label id="Var2Label" style="Z-INDEX: 111; LEFT: 376px; POSITION: absolute; TOP: 56px"
        runat="server" Height="32px" Width="488px"></asp:label><asp:label id="Label5" style="Z-INDEX: 110; LEFT: 288px; POSITION: absolute; TOP: 56px" runat="server">Var2</asp:label><asp:label id="Var1Label" style="Z-INDEX: 104; LEFT: 376px; POSITION: absolute; TOP: 24px"
        runat="server" Height="32px" Width="488px"></asp:label><asp:button id="Stay" style="Z-INDEX: 102; LEFT: 56px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Stay"></asp:button><asp:button id="Next" style="Z-INDEX: 103; LEFT: 192px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Next"></asp:button>
      <DIV style="WIDTH: 16px; HEIGHT: 16.5em"></DIV>
      <asp:Label id="RetValLabel" style="Z-INDEX: 114; LEFT: 376px; POSITION: absolute; TOP: 88px"
        runat="server" Width="488px"></asp:Label>
      <asp:Label id="Label1" style="Z-INDEX: 113; LEFT: 288px; POSITION: absolute; TOP: 88px" runat="server">RetVal</asp:Label>
      <uc1:UserControl1 id="UserControl11" runat="server"></uc1:UserControl1>
      <asp:Button id="Throw" style="Z-INDEX: 112; LEFT: 56px; POSITION: absolute; TOP: 192px" runat="server"
        Text="Throw"></asp:Button>
      <asp:label id="Label3" style="Z-INDEX: 109; LEFT: 280px; POSITION: absolute; TOP: 128px" runat="server">Stack</asp:label><asp:label id="Label2" style="Z-INDEX: 108; LEFT: 288px; POSITION: absolute; TOP: 24px" runat="server">Var1</asp:label><asp:label id="StackLabel" style="Z-INDEX: 107; LEFT: 376px; POSITION: absolute; TOP: 128px"
        runat="server" Height="168px" Width="480px"></asp:label><asp:button id="Sub" style="Z-INDEX: 106; LEFT: 128px; POSITION: absolute; TOP: 144px" runat="server"
        Text="Sub"></asp:button><asp:checkbox id="IsPostBackCheck" style="Z-INDEX: 105; LEFT: 88px; POSITION: absolute; TOP: 96px"
        runat="server" Text="IsPostBack" Enabled="False"></asp:checkbox>
      <rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager>
      <TABLE id="FormGrid" style="Z-INDEX: 115; LEFT: 904px; WIDTH: 300px; POSITION: absolute; TOP: 80px; HEIGHT: 176px"
        cellSpacing="1" cellPadding="1" width="300" border="1" runat="server">
        <TR>
          <TD></TD>
          <TD></TD>
          <TD></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD></TD>
          <TD></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD></TD>
          <TD></TD>
        </TR>
      </TABLE>
    </FORM>
  </body>
</HTML>
