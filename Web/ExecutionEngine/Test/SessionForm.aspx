<%@ Page language="c#" Codebehind="SessionForm.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.SessionForm" %>
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
    <rubicon:WebButton id="PostBackButton" runat="server" Text="PostBack"></rubicon:WebButton><rubicon:WebButton id="WebButton1" runat="server"></rubicon:WebButton><asp:LinkButton id="LinkButton1" runat="server">LinkButton</asp:LinkButton>
    <a id="LinkButton2" href="#" onclick="__doPostBack('LinkButton1',''); return false;">LinkButton 2</a>
    <div id="WaitMessage" 
    style="BORDER: thin solid; 
      Z-INDEX: 10; 
      LEFT: 20%; 
      VISIBILITY: hidden; 
      COLOR: white; 
      FONT-FAMILY: Arial; 
      POSITION: absolute; 
      TOP: 20pt; 
      BACKGROUND-COLOR: blue; 
      TEXT-ALIGN: center;
      MARGIN: 10pt;
      PADDING: 10pt;">
    Eine Interaktion mit dem Server läuft gerade.<br>

    Bitte warten Sie, bis die Antwort vom Server kommt. 
    </div></form>
    <script language="javascript">
    </script>
  </body>
</html>
