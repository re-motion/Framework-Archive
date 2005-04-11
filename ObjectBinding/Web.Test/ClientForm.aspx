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
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>

<script language="javascript">
  var _keepAliveLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormKeepAliveWxeFunction,OBWTest';
  var aktiv = window.setInterval('KeepAlive()', 5000);
  
  function KeepAlive()
  {
    try 
    {
      var keepAliveImage = new Image();
      keepAliveImage.src = _keepAliveLocation;
    }
    catch (e)
    {
    }
  }
</script>
  
<script language="javascript">
  var _expiredLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormClosingWxeFunction,OBWTest';
  
  function OnUnload()
  {
    window.document.location = _expiredLocation;
  }
  function OnBeforeUnload()
  {
    var isOutsideClientLeft = event.clientX < 0;
    var isOutsideClientTop = event.clientY < 0;
    var isOutsideClientRight = event.clientX > (window.document.body.clientLeft + window.document.body.clientWidth);
    var isOutsideClientBottom = event.clientY > (window.document.body.clientTop + window.document.body.clientHeight);
    var isOutsideClient = isOutsideClientLeft || isOutsideClientTop || isOutsideClientRight || isOutsideClientBottom;
    
    if (isOutsideClient)
    {
      //debugger;
      // Catches menubar clicks, location clicks, close-button clicks.
      //event.returnValue = 'Are you sure you want to leave the page?';
      window.document.body.onunload = OnUnload;
    }
  }

  function OnKeyDown()
  {
    if (event.keyCode == 116)
    {
      location.replace (_expiredLocation);
    }
  }
</script>
  </head>
<body MS_POSITIONING="FlowLayout" onBeforeUnload="OnBeforeUnload();" onkeydown="OnKeyDown();">
    <form id=Form method=post runat="server">
      <rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
      </rubicon:tabbedmultiview>
    </form>
  </body>
</html>
