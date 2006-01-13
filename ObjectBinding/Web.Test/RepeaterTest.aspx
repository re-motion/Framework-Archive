<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonDetailsUserControl" Src="TestTabbedPersonDetailsUserControl.ascx" %>
<%@ Page language="c#" Codebehind="RepeaterTest.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.RepeaterTest" %>
<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonJobsUserControl" Src="TestTabbedPersonJobsUserControl.ascx" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="cc1" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>RepeaterTest</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form1 method=post runat="server"><rubicon:WebButton id="SaveButton" runat="server" Text="Save"></rubicon:WebButton><asp:repeater id=Repeater1 runat="server">
    <itemtemplate>
    <div>
    <obw:boctextvalue id="UnboundField" runat="server">
</obw:boctextvalue>
</div>
</itemtemplate>
</asp:repeater>
<hr>
<obrt:objectboundrepeater id=Repeater2 runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children">
<itemtemplate>
    <div>
    <obw:boctextvalue id="FirstNameField" runat="server" ReadOnly="true" DataSourceControl="ItemDataSourceControl" PropertyIdentifier="FirstName">
</obw:boctextvalue>
<cc1:reflectionbusinessobjectdatasourcecontrol id="ItemDataSourceControl" runat="server" typename="OBRTest.Person,OBRTest"></cc1:reflectionbusinessobjectdatasourcecontrol>
</div>
</ItemTemplate>
</obrt:objectboundrepeater>
<hr>
<obrt:objectboundrepeater id="Repeater3" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Children" readonly="True">
<itemtemplate>
<table style="width:100%">
<tr>
<td>
<uc1:testtabbedpersondetailsusercontrol id="TestTabbedPersonDetailsUserControl1" runat="server"></uc1:testtabbedpersondetailsusercontrol>
</td>
<td>
<uc1:testtabbedpersonjobsusercontrol id="TestTabbedPersonJobsUserControl1" runat="server"></uc1:testtabbedpersonjobsusercontrol>
</td></tr>
</table>
</ItemTemplate>
</obrt:objectboundrepeater>
<hr>
<cc1:reflectionbusinessobjectdatasourcecontrol id="CurrentObject" runat="server" typename="OBRTest.Person,OBRTest"></cc1:reflectionbusinessobjectdatasourcecontrol>
	</form>
  </body>
</html>
