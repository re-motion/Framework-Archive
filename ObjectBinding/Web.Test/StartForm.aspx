<%@ Page language="c#" Codebehind="StartForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.StartForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Start Form</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="FlowLayout">

    <form id="Form" method="post" runat="server">
      <p>Wxe-Enabled Unit Tests<br>
        <a href="WxeHandler.ashx?WxeFunctionType=OBWTest.UnitTestMainWxeFunction,OBWTest">
          WxeHandler.ashx?WxeFunctionType=OBWTest.UnitTestMainWxeFunction,OBWTest</a></p>
      <p>Wxe-Enabled Integration Test (Single Page, no User Control)<br>
        <a href="WxeHandler.ashx?WxeFunctionType=OBWTest.IntegrationTestMainWxeFunction,OBWTest">
          WxeHandler.ashx?WxeFunctionType=OBWTest.IntegrationTestMainWxeFunction,OBWTest</a></p>
    </form>

  </body>
</html>
