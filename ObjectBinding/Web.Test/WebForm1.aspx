<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web.UI" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm1" %>
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
      <obc:boctextvalue id=FirstNameField style="Z-INDEX: 101; LEFT: 224px; POSITION: absolute; TOP: 16px" runat="server" DataSource="<%# reflectionBusinessObjectDataSource1 %>" PropertyIdentifier="FirstName">
        <TextBoxStyle AutoPostBack="True" CssClass="MyCssClass"></TextBoxStyle>
      </obc:boctextvalue>
      <rwc:SmartLabel id="BocPropertyLabel1" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 16px"
        runat="server" ForControl="FirstNameField"></rwc:SmartLabel>
      <obc:boctextvalue id=LastNameField style="Z-INDEX: 104; LEFT: 224px; POSITION: absolute; TOP: 48px" runat="server" PropertyIdentifier="LastName" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel2" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 48px"
        runat="server" ForControl="LastNameField"></rwc:SmartLabel><obc:boctextvalue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 224px; POSITION: absolute; TOP: 80px" runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# reflectionBusinessObjectDataSource1 %>" ValueType="Date">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 80px"
        runat="server" ForControl="DateOfBirthField"></rwc:SmartLabel><obc:boctextvaluevalidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 424px; POSITION: absolute; TOP: 80px"
        runat="server" ControlToValidate="DateOfBirthField" EnableClientScript="False"></obc:boctextvaluevalidator><obc:boctextvalue id=HeightField style="Z-INDEX: 109; LEFT: 224px; POSITION: absolute; TOP: 112px" runat="server" PropertyIdentifier="Height" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel4" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 112px"
        runat="server" ForControl="HeightField"></rwc:SmartLabel><obc:boctextvaluevalidator id="BocTextValueValidator2" style="Z-INDEX: 111; LEFT: 424px; POSITION: absolute; TOP: 112px"
        runat="server" ControlToValidate="HeightField" EnableClientScript="False"></obc:boctextvaluevalidator><asp:label id="Label1" style="Z-INDEX: 112; LEFT: 384px; POSITION: absolute; TOP: 112px" runat="server">cm</asp:label><asp:button id="SaveButton" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 168px"
        runat="server" Text="Save" Width="80px"></asp:button>
      <obc:BocEnumValue id=GenderField style="Z-INDEX: 113; LEFT: 224px; POSITION: absolute; TOP: 224px" runat="server" PropertyIdentifier="MarriageStatus" DataSource="<%# reflectionBusinessObjectDataSource1 %>" Width="144px" Height="104px">
        <LISTCONTROLSTYLE RadioButtonListRepeatDirection="Horizontal" ControlType="ListBox" BackColor="#FFFF80"
          RadionButtonListRepeatLayout="Table" ForeColor="Red" BorderColor="Red" Font-Bold="True" RadioButtonListTextAlign="Right"></LISTCONTROLSTYLE>
      </obc:BocEnumValue>
      <rwc:SmartLabel id="BocPropertyLabel5" style="Z-INDEX: 114; LEFT: 32px; POSITION: absolute; TOP: 224px"
        runat="server" ForControl="GenderField"></rwc:SmartLabel>
      <asp:TextBox id="TextBox1" style="Z-INDEX: 115; LEFT: 664px; POSITION: absolute; TOP: 352px"
        runat="server"></asp:TextBox>
      <asp:ListBox id="ListBox1" style="Z-INDEX: 116; LEFT: 144px; POSITION: absolute; TOP: 448px"
        runat="server"></asp:ListBox>
      <asp:RadioButtonList id="RadioButtonList1" style="Z-INDEX: 117; LEFT: 360px; POSITION: absolute; TOP: 432px"
        runat="server"></asp:RadioButtonList></form>
  </body>
</HTML>
