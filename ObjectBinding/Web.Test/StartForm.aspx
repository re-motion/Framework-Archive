<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="StartForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.StartForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Start Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
<script language=javascript>
function OpenClientWindow (url)
{
  var clientWindow = window.open (url, 'ClientWindow', 'menubar=yes,toolbar=yes,location=yes,status=yes');
}
function OnUnloadWindow()
{
  //debugger;
}
function OnBeforeUnloadWindow()
{
  //debugger;
}
</script>
</head>
<body onbeforeunload=OnBeforeUnloadWindow(); onunload=OnUnloadWindow(); MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server">
<p>Wxe-Enabled Tests for individual Business Object 
Controls<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest</A></p>
<p>Wxe-Enabled Tests containing all the Business Object 
Controls in a single Form or User Control<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest</A></p>
<p>Wxe-Enabled Test for a Tabbed Form<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false" >WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false</A></p>
<p>Test Tree View<br><A href="SingleTestTreeView.aspx" >SingleTestTreeView.aspx</A></p>
<p><A href="javascript:OpenClientWindow ('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=false');" >OpenClientWindow 
('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=false')</A></p>
<p><a href="javascript:OpenClientWindow ('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=true');" >OpenClientWindow 
('WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormWxeFunction,OBWTest&amp;ReadOnly=true')</a></p>
<p><a 
href="javascript:OpenClientWindow ('ClientFormFrameset.htm');">OpenClientWindow 
('ClientFormFrameset.htm')</a></p>
<p>&nbsp;</p></form>
  </body>
</html>
