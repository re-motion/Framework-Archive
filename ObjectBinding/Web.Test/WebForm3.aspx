<%@ Page language="c#" Codebehind="WebForm3.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>WebForm3</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
      <label for="RadioButtonList1" accesskey="l"><u>L</u>abel f�r RadioButtonList1</label>
      <rubicon:BocTextValue id=FirstNameField style="Z-INDEX: 101; LEFT: 248px; POSITION: absolute; TOP: 48px" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="FirstName">
      </rubicon:BocTextValue>
      <rubicon:SmartLabel id="BocPropertyLabel1" style="Z-INDEX: 102; LEFT: 48px; POSITION: absolute; TOP: 48px"
        runat="server" ForControl="FirstNameField"></rubicon:SmartLabel>
      <asp:Button id="SaveButton" style="Z-INDEX: 103; LEFT: 48px; POSITION: absolute; TOP: 144px"
        runat="server" Text="Save" Width="144px"></asp:Button>
      <cc1:BocTextValue id="HeightField" style="Z-INDEX: 104; LEFT: 248px; POSITION: absolute; TOP: 80px"
        runat="server" PropertyIdentifier="Height" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:BocTextValue>
      <rubicon:SmartLabel id="BocPropertyLabel2" style="Z-INDEX: 105; LEFT: 48px; POSITION: absolute; TOP: 88px"
        runat="server" ForControl="HeightField"></rubicon:SmartLabel>
      <cc1:BocTextValueValidator id="BocTextValueValidator1" style="Z-INDEX: 106; LEFT: 432px; POSITION: absolute; TOP: 80px"
        runat="server" ControlToValidate="HeightField"></cc1:BocTextValueValidator>
      <asp:RadioButtonList id="RadioButtonList1" style="Z-INDEX: 107; LEFT: 288px; POSITION: absolute; TOP: 264px"
        runat="server">
        <asp:ListItem Value="asd">asd</asp:ListItem>
        <asp:ListItem Value="asdasd">asdasd</asp:ListItem>
      </asp:RadioButtonList>
    </form>
  </body>
</HTML>
