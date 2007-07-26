

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
      <rubicon:bindableobjectdatasourcecontrol id="CurrentObjectDataSource" runat="server" TypeName="Rubicon.ObjectBinding.Sample::Person"/>
      <rubicon:businessobjectreferencedatasourcecontrol id="PartnerDataSource" runat="server" PropertyIdentifier="Partner" ReferencedDataSource="CurrentObjectDataSource"
        DataSourceControl="CurrentObjectDataSource"></rubicon:businessobjectreferencedatasourcecontrol>
      <rubicon:boctextvalue id=FirstNameField style="Z-INDEX: 100; LEFT: 224px; POSITION: absolute; TOP: 56px" runat="server" PropertyIdentifier="FirstName" DataSource="<%# CurrentObjectDataSource %>">
        <textboxstyle autopostback="True" cssclass="MyCssClass"></textboxstyle>
      </rubicon:boctextvalue><rubicon:smartlabel id="SmartLabel2" style="Z-INDEX: 119; LEFT: 696px; POSITION: absolute; TOP: 360px"
        runat="server" ForControl="PartnerFirstNameField"></rubicon:smartlabel><rubicon:smartlabel id="BocPropertyLabel1" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 56px"
        runat="server" ForControl="FirstNameField"></rubicon:smartlabel><rubicon:boctextvalue id=LastNameField style="Z-INDEX: 103; LEFT: 224px; POSITION: absolute; TOP: 104px" runat="server" PropertyIdentifier="LastName" DataSource="<%# CurrentObjectDataSource %>">
      </rubicon:boctextvalue><rubicon:smartlabel id="BocPropertyLabel2" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 104px"
        runat="server" ForControl="LastNameField"></rubicon:smartlabel><rubicon:boctextvalue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 224px; POSITION: absolute; TOP: 136px" runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# CurrentObjectDataSource %>" ValueType="Date">
      </rubicon:boctextvalue><rubicon:smartlabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 136px"
        runat="server" ForControl="DateOfBirthField"></rubicon:smartlabel><rubicon:boctextvaluevalidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 480px; POSITION: absolute; TOP: 136px"
        runat="server" EnableClientScript="False" ControlToValidate="DateOfBirthField"></rubicon:boctextvaluevalidator><rubicon:boctextvalue id=HeightField style="Z-INDEX: 109; LEFT: 224px; POSITION: absolute; TOP: 168px" runat="server" PropertyIdentifier="Height" DataSource="<%# CurrentObjectDataSource %>">
      </rubicon:boctextvalue><rubicon:smartlabel id="BocPropertyLabel4" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 168px"
        runat="server" ForControl="HeightField"></rubicon:smartlabel><rubicon:boctextvaluevalidator id="BocTextValueValidator2" style="Z-INDEX: 111; LEFT: 480px; POSITION: absolute; TOP: 168px"
        runat="server" EnableClientScript="False" ControlToValidate="HeightField"></rubicon:boctextvaluevalidator><asp:label id="Label1" style="Z-INDEX: 112; LEFT: 440px; POSITION: absolute; TOP: 168px" runat="server">cm</asp:label><asp:button id="SaveButton" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 392px"
        runat="server" Width="80px" Text="Save"></asp:button><rubicon:bocenumvalue id=GenderField style="Z-INDEX: 113; LEFT: 224px; POSITION: absolute; TOP: 216px" runat="server" PropertyIdentifier="Gender" DataSource="<%# CurrentObjectDataSource %>" Width="152px" Height="24px">
        <listcontrolstyle radiobuttonlisttextalign="Right" font-bold="True" bordercolor="Red" forecolor="Green"
          radionbuttonlistrepeatlayout="Table" backcolor="#FFFF80" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </rubicon:bocenumvalue><rubicon:smartlabel id="BocPropertyLabel5" style="Z-INDEX: 114; LEFT: 16px; POSITION: absolute; TOP: 216px"
        runat="server" ForControl="GenderField"></rubicon:smartlabel><rubicon:bocenumvalue id=MarriageStatusField style="Z-INDEX: 115; LEFT: 224px; POSITION: absolute; TOP: 320px" runat="server" PropertyIdentifier="MarriageStatus" DataSource="<%# CurrentObjectDataSource %>" Width="152px">
        <listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList"
          radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </rubicon:bocenumvalue><rubicon:smartlabel id="SmartLabel1" style="Z-INDEX: 116; LEFT: 24px; POSITION: absolute; TOP: 328px"
        runat="server" ForControl="MarriageStatusField" Width="120px" Height="8px"></rubicon:smartlabel><rubicon:boctextvalue id=PartnerFirstNameField style="Z-INDEX: 117; LEFT: 904px; POSITION: absolute; TOP: 360px" runat="server" PropertyIdentifier="FirstName" DataSource="<%# PartnerDataSource %>">
      </rubicon:boctextvalue><asp:label id="Label2" style="Z-INDEX: 118; LEFT: 696px; POSITION: absolute; TOP: 328px" runat="server"
        Font-Bold="True">Partner</asp:label></form>
  </body>
</HTML>
