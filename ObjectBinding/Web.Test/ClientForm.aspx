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
  var active = window.setInterval('KeepAlive()', 180000);
  
  function KeepAlive()
  {
    try 
    {
      var image = new Image();
      image.src = _keepAliveLocation;
    }
    catch (e)
    {
    }
  }
</script>
  
<script language="javascript">
  var _wxe_expiredLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormClosingWxeFunction,OBWTest';
  var _wxe_isSubmit = false;
  var _wxe_aspnetDoPostBack = null;
  
  function OnBeforeUnload()
  {
    if (! _wxe_isSubmit)
    {
      event.returnValue = "If you leave now, forever lost your session will be.";
      event.cancelBubble = true;
    }
  }

  function OnUnload()
  {
    if (! _wxe_isSubmit)
    {
      try 
      {
        var image = new Image();
        image.src = _wxe_expiredLocation;
      }
      catch (e)
      {
      }
    }
  }
  
  function OnLoad()
  {
    var theform;
		if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1)
			theform = document.Form;
		else 
			theform = document.forms["Form"];
	  theform.onsubmit = function() { _wxe_isSubmit = true; };
	  
	  _wxe_aspnetDoPostBack = __doPostBack;
	  __doPostBack = function (eventTarget, eventArgument)
	  {
	    _wxe_isSubmit = true;
	    _wxe_aspnetDoPostBack (eventTarget, eventArgument);
	  }
  }
  
</script>
  </head>
<body MS_POSITIONING="FlowLayout" onLoad="OnLoad();" onBeforeUnload="OnBeforeUnload();" onUnload="OnUnload();" >
    <form id=Form method=post runat="server">
      <rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
      </rubicon:tabbedmultiview>
    </form>
  </body>
</html>
