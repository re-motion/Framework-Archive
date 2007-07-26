<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonDetailsUserControl" Src="TestTabbedPersonDetailsUserControl.ascx" %>
<%@ Page language="c#" Codebehind="RepeaterTest.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.RepeaterTest" %>
<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonJobsUserControl" Src="TestTabbedPersonJobsUserControl.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>RepeaterTest</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form1 method=post runat="server"><rubicon:webbutton id=SaveButton runat="server" Text="Save"></rubicon:webbutton>
<ros:ObjectBoundRepeater id=Repeater2 runat="server" propertyidentifier="Children" datasourcecontrol="CurrentObject">
<itemtemplate>
    <div>
    <rubicon:boctextvalue id="FirstNameField" runat="server" ReadOnly="true" DataSourceControl="ItemDataSourceControl" PropertyIdentifier="FirstName">
</rubicon:boctextvalue>
<rubicon:bindableobjectdatasourcecontrol id="ItemDataSourceControl" runat="server" typename="Rubicon.ObjectBinding.Sample::Person"/>
</div>
</ItemTemplate>
</ros:ObjectBoundRepeater>
<hr>
<ros:ObjectBoundRepeater id=Repeater3 runat="server" propertyidentifier="Children" datasourcecontrol="CurrentObject">
<itemtemplate>
<table style="width:100%">
<tr>
<td>
<!--DataEditUserControl-->
<uc1:testtabbedpersondetailsusercontrol id="TestTabbedPersonDetailsUserControl1" runat="server"></uc1:testtabbedpersondetailsusercontrol>
</td>
<td>
<!--DataEditUserControl-->
<uc1:testtabbedpersonjobsusercontrol id="TestTabbedPersonJobsUserControl1" runat="server"></uc1:testtabbedpersonjobsusercontrol>
</td></tr>
</table>
</ItemTemplate>
</ros:ObjectBoundRepeater>
<hr>
<rubicon:bindableobjectdatasourcecontrol id=CurrentObject runat="server" typename="Rubicon.ObjectBinding.Sample::Person"/></form>
  </body>
</html>
