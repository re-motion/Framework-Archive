<%@ Page language="c#" Codebehind="WebForm3.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm3" %>
<%@ Register TagPrefix="cc1" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
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
      <cc1:BocTextValue id=FirstNameField style="Z-INDEX: 101; LEFT: 248px; POSITION: absolute; TOP: 48px" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="FirstName">
      </cc1:BocTextValue>
      <cc1:BocPropertyLabel id="BocPropertyLabel1" style="Z-INDEX: 102; LEFT: 48px; POSITION: absolute; TOP: 48px"
        runat="server" ForControl="FirstNameField"></cc1:BocPropertyLabel>
      <asp:Button id="SaveButton" style="Z-INDEX: 103; LEFT: 48px; POSITION: absolute; TOP: 144px"
        runat="server" Text="Save" Width="144px"></asp:Button>
      <cc1:BocTextValue id="HeightField" style="Z-INDEX: 104; LEFT: 248px; POSITION: absolute; TOP: 80px"
        runat="server" PropertyIdentifier="Height" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:BocTextValue>
      <cc1:BocPropertyLabel id="BocPropertyLabel2" style="Z-INDEX: 105; LEFT: 48px; POSITION: absolute; TOP: 88px"
        runat="server" ForControl="HeightField"></cc1:BocPropertyLabel>
      <cc1:BocTextValueValidator id="BocTextValueValidator1" style="Z-INDEX: 106; LEFT: 432px; POSITION: absolute; TOP: 80px"
        runat="server" ControlToValidate="HeightField"></cc1:BocTextValueValidator>
    </form>
  </body>
</HTML>
