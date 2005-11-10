<%@ Page language="c#" Codebehind="SessionForm.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.SessionForm" smartNavigation="False"%>
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
</script>
</head>
<body MS_POSITIONING="FlowLayout">
    <form id=Form method=post runat="server">
<table style="WIDTH:100%; HEIGHT:100%">
<tr>
<td style="VERTICAL-ALIGN:top">
    <p><asp:label id="FunctionTokenLabel" runat="server">Token</asp:label>, 
    <asp:label id="PostBackIDLabel" runat="server">PostBackID</asp:label></p>
    <rubicon:WebButton id="PostBackButton" runat="server" Text="PostBack"></rubicon:WebButton><rubicon:WebButton id="OpenSelfButton" runat="server" Text="Open Self"></rubicon:WebButton> 
    <asp:linkbutton id="LinkButton1" runat="server">LinkButton</asp:linkbutton>
    <a id="LinkButton2" href="#" onclick="__doPostBack('LinkButton1',''); return false;">LinkButton 2</a>
<p>
<asp:button id="Button1" runat="server" Text="Button"></asp:button>
</p><p>
<rubicon:webbutton id="Button1Button" runat="server" Text="Button 1" UseSubmitBehavior="False"></rubicon:webbutton>
</p><p>
<rubicon:webbutton id="Submit1Button" runat="server" Text="Submit 1"></rubicon:webbutton>
</p><p>
<rubicon:webbutton id="ExecuteButton" runat="server" Text="Execute"></rubicon:webbutton>
</p><p>
<rubicon:webbutton id="ExecuteNoRepostButton" runat="server" Text="Execute, No Repost"></rubicon:webbutton>
</p><p>
<rubicon:webbutton id="Button2Button" runat="server" Text="Button 2" UseSubmitBehavior="False"></rubicon:webbutton>
</p>
<p><rubicon:WebButton id="OpenSampleFunctionButton" runat="server" Text="Open Sample Function"></rubicon:WebButton><br>
<rubicon:WebButton id="OpenSampleFunctionWithPermanentUrlButton" runat="server" Text="Open Sample Function with Permant URL"></rubicon:WebButton><br>
<rubicon:WebButton id="OpenSampleFunctionInNewWindowButton" runat="server" Text="Open Sample Function in New Window"></rubicon:WebButton><br>
<rubicon:WebButton id="OpenSampleFunctionWithPermanentUrlInNewWindowButton" runat="server" Text="Open Sample Function with Permanent URL in New Window"></rubicon:WebButton><br><rubicon:WebButton id="OpenSessionFunctionButton" runat="server" Text="Open Session Function"></rubicon:WebButton><br><rubicon:WebButton id="OpenSessionFunctionWithPermanentUrlButton" runat="server" Text="Open Session Function with Permanent URL"></rubicon:WebButton><br>
<rubicon:WebButton id="OpenSessionFunctionInNewWindowButton" runat="server" Text="Open Session Function in New Window"></rubicon:WebButton><br>
<rubicon:WebButton id="OpenSessionFunctionWithPermanentUrlInNewWindowButton" runat="server" Text="Open Session Function with Permanent URL in New Window"></rubicon:WebButton><br>
Permalink this: <asp:HyperLink id="CurrentFunctionPermaLink" runat="server"></asp:HyperLink><br>Permalink 
Sample: <asp:HyperLink id="SampleFunctionPermaLink" runat="server"></asp:HyperLink></p>

</td>
</tr>
</table>
</form>
    <script language="javascript">
    function Page_PostBack (eventTarget, eventArgs)
    {
    }

    function Page_Abort ()
    {
    }
    
    function Page_Load ()
    {
      /*
      // IE Only
      var windowTop = 25 + 24 + 24 + 24; // Title bar, menu bar, stanarddbuttons bar, address bar
      var windowBottom = 24;
      var windowLeft = 4;
      var windowRight = 4;
      
      _width = document.body.offsetWidth + windowLeft + windowRight;
      _height = document.body.offsetHeight + windowTop + windowBottom;
      _left = window.screenLeft - windowLeft;
      _top = window.screenTop - windowTop;

      var newWidth = 300;
      var newHeight = 150;
      window.resizeTo (newWidth, newHeight);
      
      var newLeft = ((screen.width - document.body.clientWidth - windowLeft - windowRight) / 2)
      var newTop = ((screen.height - document.body.clientHeight - windowTop - windowBottom) / 2);      
      window.moveTo (newTop, _top);
      */
    }
    
    function Page_BeforeUnload ()
    {
    }
    
    function Page_Unload ()
    {
      /*
      window.resizeTo (_width, _height);
      window.moveTo (_left, _top);
      */
    }
    </script>
  </body>
</html>
