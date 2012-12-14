<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="SingleBocTestEnumValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleBocEnumValueForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: EnumValue Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
  <body>
    <form id="Form" method="post" runat="server">
<h1>SingleBocTest: EnumValue Form</h1>
      <table id="FormGrid" runat="server">
        <tr>
          <td colSpan="4"><obc:boctextvalue id="FirstNameField" runat="server" readonly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              PropertyIdentifier="FirstName"></obc:boctextvalue>&nbsp;<obc:boctextvalue id="LastNameField" runat="server" readonly="True" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              PropertyIdentifier="LastName"></obc:boctextvalue></td>
        </tr>
        <tr>
          <td></td>
          <td>
            <obc:bocenumvalue id="GenderField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              PropertyIdentifier="Gender" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
            </obc:bocenumvalue></td>
          <td>
            <p>bound, radio buttons, required=true</p>
          </td>
    <td style="WIDTH: 20%"><asp:label id="GenderFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <tr>
          <td></td>
          <td>
            <obc:bocenumvalue id="ReadOnlyGenderField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              PropertyIdentifier="Gender" Required="True" ReadOnly="True">
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" controltype="RadioButtonList"
                radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding=""
                radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </obc:bocenumvalue></td>
          <td>
            <p>bound, read-only</p>
          </td>
    <td style="WIDTH: 20%"><asp:label id="ReadOnlyGenderFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <tr>
          <td></td>
          <td>
            <obc:bocenumvalue id="MarriageStatusField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              PropertyIdentifier="MarriageStatus" required="False">
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" radiobuttonlistrepeatdirection="Horizontal"
                radiobuttonlistcellspacing="" radiobuttonlistcellpadding="" radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </obc:bocenumvalue></td>
          <td>
            <p>bound, drop-down, required=false</p>
          </td>
    <td style="WIDTH: 20%"><asp:label id="MarriageStatusFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <tr>
          <td></td>
          <td>
            <obc:BocEnumValue id="UnboundMarriageStatusField" runat="server">
              <listcontrolstyle radiobuttonlisttextalign="Right" listboxrows="2" radionbuttonlistrepeatlayout="Table"
                controltype="ListBox" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
            </obc:BocEnumValue></td>
          <td>
            <p>unbound, value not set, list-box, required=true</p>
          </td>
    <td style="WIDTH: 20%"><asp:label id="UnboundMarriageStatusFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <tr>
          <td></td>
          <td>
            <obc:BocEnumValue id="UnboundReadOnlyMarriageStatusField" runat="server" ReadOnly="True">
              <listcontrolstyle autopostback="" listboxrows="" radionbuttonlistrepeatlayout="Table" controltype="ListBox"
                radiobuttonlistcellspacing="" radiobuttonlistcellpadding="" radiobuttonlistrepeatcolumns=""></listcontrolstyle>
            </obc:BocEnumValue></td>
          <td>
            <p>unbound, value set, read only</p>
          </td>
    <td style="WIDTH: 20%"><asp:label id="UnboundReadOnlyMarriageStatusFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </tr>
        <TR>
          <TD></TD>
          <TD>
            <obc:bocenumvalue id="DeceasedAsEnumField" runat="server" PropertyIdentifier="Deceased" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"
              required="False">
              <listcontrolstyle radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding=""></ListControlStyle>
            </obc:bocenumvalue></TD>
          <TD>deceased (bool) as enum</TD>
    <td style="WIDTH: 20%"><asp:label id="DeceasedAsEnumFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td>
        </TR>
      </table>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
<p><br>
      Gender Selection Changed: <asp:Label id="GenderFieldSelectionChangedLabel" runat="server" EnableViewState="False">#</asp:Label></p>
      <p><asp:button id="GenderTestSetNullButton" runat="server" Text="Gender Set Null" width="165px"></asp:button><asp:button id="GenderTestSetDisabledGenderButton" runat="server" Text="Gender Set Disabled Gender"
          width="165px"></asp:button><asp:button id="GenderTestSetMarriedButton" runat="server" Text="Gender Set Married" width="165px"></asp:button></p>
      <p><br>
        <asp:button id="ReadOnlyGenderTestSetNullButton" runat="server" Text="Read Only Gender Set Null"
          width="220px"></asp:button><asp:button id="ReadOnlyGenderTestSetNewItemButton" runat="server" Text="Read Only Gender Set Female"
          width="220px"></asp:button></p>
      <p><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
    </form>
  </body>
</html>
