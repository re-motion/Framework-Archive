<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Templates.Generic.Web.Default" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!doctype HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <title>Start</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
    <script language="javascript" type="text/javascript">
	    function StartApp ()
	    { 
	      <%-- Set height so that IE window fits for 1024x768 with windows taskbar --%>
	      var windowMaxWidth = 1010;
	      var windowMaxHeight = 700;
	      var windowLeft = (screen.width - windowMaxWidth) / 2;
	      
	      var windowTop;
	      if (screen.height > 768)
	      {
	        <%-- Center window vertical for resolutions greater than 1024x768 --%>  
	        windowTop = (screen.height - windowMaxHeight) / 2;
	      }
        else  	      
        {
          <%-- Do not center window because of window taskbar --%>  
	        windowTop = 5; 
	      }   
	      
        window.open ("WxeHandler.ashx?WxeFunctionType=Rubicon.Templates.Generic.Web.WxeFunctions.TemplateFunction,Rubicon.Templates.Generic.Web", "_blank",
			      "width=" + windowMaxWidth + ",height=" + windowMaxHeight + ",top=" + windowTop + ",left=" + windowLeft + "," + 
			      "resizable=yes,location=yes,menubar=no,status=no,toolbar=no,scrollbars=no" ); <%-- use "location=no" in production environments --%>
	    }	    
    </script>
</head>
  <body MS_POSITIONING="FlowLayout" onload="StartApp();">
    <form id="defaultForm" method="post" runat="server">
      <div id="PageHeader">
        <img src="Image/rublogo.gif" alt="RUBICON">
      </div>
        <h1>Template Application</h1>
        <p>
          Use the following link to start the template application:
          <br>
          <img class="pfeil" src="Image/arrow.gif" alt="">&nbsp;<asp:linkbutton id="StartButton" runat="server">Start Template Application</asp:linkbutton>
        </p>
    </form>
  </body>
</html>
