<%@ Register TagPrefix="cc1" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="BocTest.WebForm1" %>
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
      <cc1:boctextvalue id=FirstNameField style="Z-INDEX: 101; LEFT: 224px; POSITION: absolute; TOP: 16px" runat="server" PropertyIdentifier="FirstName" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:boctextvalue><cc1:bocpropertylabel id="BocPropertyLabel1" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 16px"
        runat="server" ForControl="FirstNameField"></cc1:bocpropertylabel><cc1:boctextvalue id=LastNameField style="Z-INDEX: 104; LEFT: 224px; POSITION: absolute; TOP: 48px" runat="server" PropertyIdentifier="LastName" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:boctextvalue><cc1:bocpropertylabel id="BocPropertyLabel2" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 48px"
        runat="server" ForControl="LastNameField"></cc1:bocpropertylabel><cc1:boctextvalue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 224px; POSITION: absolute; TOP: 80px" runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# reflectionBusinessObjectDataSource1 %>" ValueType="Date">
      </cc1:boctextvalue><cc1:bocpropertylabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 80px"
        runat="server" ForControl="DateOfBirthField"></cc1:bocpropertylabel><cc1:boctextvaluevalidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 424px; POSITION: absolute; TOP: 80px"
        runat="server" ControlToValidate="DateOfBirthField" EnableClientScript="False"></cc1:boctextvaluevalidator><cc1:boctextvalue id=HeightField style="Z-INDEX: 109; LEFT: 224px; POSITION: absolute; TOP: 112px" runat="server" PropertyIdentifier="Height" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </cc1:boctextvalue><cc1:bocpropertylabel id="BocPropertyLabel4" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 112px"
        runat="server" ForControl="HeightField"></cc1:bocpropertylabel><cc1:boctextvaluevalidator id="BocTextValueValidator2" style="Z-INDEX: 111; LEFT: 424px; POSITION: absolute; TOP: 112px"
        runat="server" ControlToValidate="HeightField" EnableClientScript="False">
        
      </cc1:boctextvaluevalidator><asp:label id="Label1" style="Z-INDEX: 112; LEFT: 384px; POSITION: absolute; TOP: 112px" runat="server">cm</asp:label><asp:button id="SaveButton" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 168px"
        runat="server" Text="Save" Width="80px"></asp:button></form>
  </body>
</HTML>
