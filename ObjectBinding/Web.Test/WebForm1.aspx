<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm1" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
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
      <obc:boctextvalue id=FirstNameField style="Z-INDEX: 100; LEFT: 224px; POSITION: absolute; TOP: 16px" runat="server" DataSource="<%# CurrentObjectDataSource %>" PropertyIdentifier="FirstName">
        <textboxstyle autopostback="True" cssclass="MyCssClass"></textboxstyle>
      </obc:boctextvalue>
      <rwc:SmartLabel id="SmartLabel2" style="Z-INDEX: 119; LEFT: 696px; POSITION: absolute; TOP: 304px"
        runat="server" ForControl="PartnerFirstNameField"></rwc:SmartLabel>
      <rwc:SmartLabel id="BocPropertyLabel1" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 16px"
        runat="server" ForControl="FirstNameField"></rwc:SmartLabel>
      <obc:boctextvalue id=LastNameField style="Z-INDEX: 103; LEFT: 224px; POSITION: absolute; TOP: 48px" runat="server" PropertyIdentifier="LastName" DataSource="<%# CurrentObjectDataSource %>">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel2" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 48px"
        runat="server" ForControl="LastNameField"></rwc:SmartLabel><obc:boctextvalue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 224px; POSITION: absolute; TOP: 80px" runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# CurrentObjectDataSource %>" ValueType="Date">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 80px"
        runat="server" ForControl="DateOfBirthField"></rwc:SmartLabel><obc:boctextvaluevalidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 424px; POSITION: absolute; TOP: 80px"
        runat="server" ControlToValidate="DateOfBirthField" EnableClientScript="False"></obc:boctextvaluevalidator><obc:boctextvalue id=HeightField style="Z-INDEX: 109; LEFT: 224px; POSITION: absolute; TOP: 112px" runat="server" PropertyIdentifier="Height" DataSource="<%# CurrentObjectDataSource %>">
      </obc:boctextvalue><rwc:SmartLabel id="BocPropertyLabel4" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 112px"
        runat="server" ForControl="HeightField"></rwc:SmartLabel><obc:boctextvaluevalidator id="BocTextValueValidator2" style="Z-INDEX: 111; LEFT: 424px; POSITION: absolute; TOP: 112px"
        runat="server" ControlToValidate="HeightField" EnableClientScript="False"></obc:boctextvaluevalidator><asp:label id="Label1" style="Z-INDEX: 112; LEFT: 384px; POSITION: absolute; TOP: 112px" runat="server">cm</asp:label><asp:button id="SaveButton" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 336px"
        runat="server" Text="Save" Width="80px"></asp:button>
      <obc:BocEnumValue id=GenderField style="Z-INDEX: 113; LEFT: 224px; POSITION: absolute; TOP: 160px" runat="server" PropertyIdentifier="Gender" DataSource="<%# CurrentObjectDataSource %>" Width="152px" Height="24px">
        <listcontrolstyle radiobuttonlisttextalign="Right" font-bold="True" bordercolor="Red" forecolor="Green"
          radionbuttonlistrepeatlayout="Table" backcolor="#FFFF80" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </obc:BocEnumValue>
      <rwc:SmartLabel id="BocPropertyLabel5" style="Z-INDEX: 114; LEFT: 16px; POSITION: absolute; TOP: 160px"
        runat="server" ForControl="GenderField"></rwc:SmartLabel>
      <obc:BocEnumValue id=MarriageStatusField style="Z-INDEX: 115; LEFT: 224px; POSITION: absolute; TOP: 264px" runat="server" PropertyIdentifier="MarriageStatus" DataSource="<%# CurrentObjectDataSource %>" Width="152px">
        <listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList"
          radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </obc:BocEnumValue>
      <rwc:SmartLabel id="SmartLabel1" style="Z-INDEX: 116; LEFT: 24px; POSITION: absolute; TOP: 272px"
        runat="server" ForControl="MarriageStatusField"></rwc:SmartLabel>
      <obc:BocTextValue id="PartnerFirstNameField" style="Z-INDEX: 117; LEFT: 904px; POSITION: absolute; TOP: 304px"
        runat="server" PropertyIdentifier="FirstName" DataSource="<%# PartnerDataSource %>">
      </obc:BocTextValue>
      <asp:Label id="Label2" style="Z-INDEX: 118; LEFT: 696px; POSITION: absolute; TOP: 272px" runat="server"
        Font-Bold="True">Partner</asp:Label></form>
  </body>
</HTML>
