<%@ Register TagPrefix="cc1" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="WebForm2.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm2" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
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
    <form id="Form1" method="post" runat="server">
      <cc1:BocTextValue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 376px; POSITION: absolute; TOP: 192px" runat="server" 
        PropertyIdentifier="DateOfBirth" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:BocTextValue>
      <rwc:SmartLabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 64px; POSITION: absolute; TOP: 192px"
        runat="server" ForControl="DateOfBirthField"></rwc:SmartLabel>
      <cc1:BocTextValueValidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 584px; POSITION: absolute; TOP: 200px"
        runat="server" ControlToValidate="DateOfBirthField"></cc1:BocTextValueValidator>
      <asp:button id="SaveButton" style="Z-INDEX: 103; LEFT: 48px; POSITION: absolute; TOP: 400px"
        runat="server" Width="80px" Text="Save"></asp:button>
    </form>
  </body>
</HTML>
