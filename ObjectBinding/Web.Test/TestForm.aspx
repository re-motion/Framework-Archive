<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
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
<table id=FormGrid runat="server" width="80%">
  <tr>
    <td></td>
    <td><obc:BocList id="BocList" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Children">
<optionsmenuitems>
<obc:BocMenuItem Icon="Open.gif" Category="Object" Text="&#214;ffnen" Command-Type="WxeFunction" Command-WxeFunctionCommand-TypeName="MyType, MyAssembly" ItemID="Open"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Copy.gif" Category="Edit" Text="Kopieren" Command-Type="Event" ItemID="Copy"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Cut.gif" Category="Edit" Text="Ausschneiden" ItemID="Cut"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Paste.gif" Category="Edit" Text="Einf&#252;gen" ItemID="Paste"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Duplicate.gif" Category="Edit" Text="Duplizieren" ItemID="Duplicate"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Delete.gif" Category="Edit" Text="L&#246;schen" ItemID="Delete"></obc:BocMenuItem>
<obc:BocMenuItem Icon="" Category="Action" Text="Link" Command-Type="Href" Command-HrefCommand-Href="link.htm" ItemID="Link"></obc:BocMenuItem>
</OptionsMenuItems>

<fixedcolumns>
<obc:BocCommandColumnDefinition Label="Cmd" Command-Type="WxeFunction" Command-WxeFunctionCommand-Parameters="@ID" Command-WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction, OBWTest" ColumnTitle="Cmd" ColumnID="Cmd"></obc:BocCommandColumnDefinition>
<obc:BocSimpleColumnDefinition PropertyPathIdentifier="FirstName"></obc:BocSimpleColumnDefinition>
</FixedColumns>

<listmenuitems>
<obc:BocMenuItem Icon="Item1.gif" Category="" Text="Item 1" ItemID="Item1"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Item2.gif" Category="" Text="Item 2" ItemID="Item2"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Item3.gif" Category="" Text="Item 3" ItemID="Item3"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Item4.gif" Category="" Text="Item 4" ItemID="Item4"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Item5.gif" Category="" Text="Item 5" ItemID="Item5"></obc:BocMenuItem>
<obc:BocMenuItem Icon="Item6.gif" Category="" Text="Item 6" ItemID="Item6"></obc:BocMenuItem>
</ListMenuItems>
</obc:BocList></td></tr>
</table>
<p>
<rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" TypeName="OBWTest.Person, OBWTest"></obr:ReflectionBusinessObjectDataSourceControl></p>
<p><asp:Button id="Button1" runat="server" Text="Post Back"></asp:Button></p></form>
	
  </body>
</html>
