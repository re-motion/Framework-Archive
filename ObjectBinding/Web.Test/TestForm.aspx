<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Test Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<table id=FormGrid runat="server">
  <tr>
    <td></td>
    <td><obc:BocList id="BocList" runat="server">
<optionsmenuitems>
<obc:BocMenuItem ID="Open" Icon="Open.gif" Category="Object" Text="&#214;ffnen"></obc:BocMenuItem>
<obc:BocMenuItem ID="Copy" Icon="Copy.gif" Category="Edit" Text="Kopieren"></obc:BocMenuItem>
<obc:BocMenuItem ID="Cut" Icon="Cut.gif" Category="Edit" Text="Ausschneiden"></obc:BocMenuItem>
<obc:BocMenuItem ID="Paste" Icon="Paste.gif" Category="Edit" Text="Einf&#252;gen"></obc:BocMenuItem>
<obc:BocMenuItem ID="Duplicate" Icon="Duplicate.gif" Category="Edit" Text="Duplizieren"></obc:BocMenuItem>
<obc:BocMenuItem ID="Delete" Icon="Delete.gif" Category="Edit" Text="L&#246;schen"></obc:BocMenuItem>
</OptionsMenuItems>

<fixedcolumns>
<obc:BocCommandColumnDefinition Label="Cmd" ColumnTitle="Cmd"></obc:BocCommandColumnDefinition>
</FixedColumns></obc:BocList></td></tr>
</table>
<rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager></form>
	
  </body>
</html>
