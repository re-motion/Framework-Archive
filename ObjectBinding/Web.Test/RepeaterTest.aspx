<%@ Page language="c#" Codebehind="RepeaterTest.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.RepeaterTest" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="cc1" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>RepeaterTest</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="FlowLayout">
	
    <form id="Form1" method="post" runat="server"><asp:Repeater id="Repeater1" runat="server"><itemtemplate><div><obw:boctextvalue id="BocTextValue1" runat="server" DataSourceControl="ItemDataSourceControl" PropertyIdentifier="FirstName">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></obw:boctextvalue><obw:BusinessObjectReferenceDataSourceControl id="ItemDataSourceControl" runat="server"></obw:BusinessObjectReferenceDataSourceControl></div></itemtemplate></asp:Repeater><cc1:ReflectionBusinessObjectDataSourceControl id="DataSource" runat="server" typename="OBRTest.Person,OBRTest"></cc1:ReflectionBusinessObjectDataSourceControl>
     </form>
	
  </body>
</html>
