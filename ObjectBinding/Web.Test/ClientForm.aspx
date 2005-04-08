<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="ClientForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.ClientForm" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>ClientForm</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
<script language="javascript">
function OpenClientWindow (url)
{
  var clientWindow = window.open (url, 'ClientWindow', 'menubar=no,toolbar=no,status=yes');
}
function OnUnloadWindow()
{
  debugger;
}
function OnBeforeUnloadWindow()
{
if (event.clientY < 0) {
      event.returnValue = 'Are you sure you want to leave the page?';
   }
   window.document
  window.document.body.onunload = OnUnloadWindow;
}
</script>
  </head>
<body MS_POSITIONING="FlowLayout" onBeforeUnload="OnBeforeUnloadWindow();">
    <form id=Form method=post runat="server">&nbsp;
    
      <rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
      </rubicon:tabbedmultiview>
    </form>
  </body>
</html>
