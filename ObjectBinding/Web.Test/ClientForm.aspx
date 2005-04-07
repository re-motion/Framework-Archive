<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="ClientForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.ClientForm" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>ClientForm</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body MS_POSITIONING="FlowLayout">
    <form id=Form method=post runat="server">
    <input type="text">    <input type="image" tabindex="1"/>

    <button type="button">button</button>
    <button type="submit" tabindex="0">submit</button>
    
      <rubicon:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
      </rubicon:tabbedmultiview>
    </form>
  </body>
</html>
