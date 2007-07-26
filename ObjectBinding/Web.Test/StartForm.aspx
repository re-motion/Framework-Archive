<%@ Page Trace="false" language="c#" Codebehind="StartForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.StartForm" %>


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
</script>
</head>
<body MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server">
<script language=javascript>
document.onkeydown = document.onkeypress = function (evt) 
{
  if (typeof evt == 'undefined')
  {
    evt = window.event;
  }
  if (evt) 
  {
    var keyCode = evt.keyCode ? evt.keyCode : evt.charCode;
    if (keyCode == 8)
    {
      if (evt.preventDefault) 
      {
         evt.preventDefault();
      }
      return false;
    }
    else 
    {
      return true;
    }
  }
  else 
  {
    return true;
  }
}
</script>
<p>Wxe-Enabled Tests for individual Business Object Controls<br>
<a href="IndividualControlTest.wxe?UserControl=BocListAsGridUserControl.ascx&WxeReturnToSelf=True&TabbedMenuSelection=IndividualControlTests,BocListAsGrid">IndividualControlTest.wxe</a></p>

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
<p>Design Test<br><a 
href="WxeHandler.ashx?WxeFunctionType=OBWTest.Design.DesignTestFunction,OBWTest">WxeHandler.ashx?WxeFunctionType=OBWTest.Design.DesignTestFunction,OBWTest</a></p>
</form>
  </body>
</html>
