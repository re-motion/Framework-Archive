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
      <p>Wxe-Enabled Tests for individual Business Object Controls<br>
        <a href="WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest">
          WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest</a></p>
      <p>Wxe-Enabled Tests containing all the Business Object 
Controls in a single Form or User Control<br>
        <a href="WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest">
          WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest</a></p>
      <p>Wxe-Enabled Test for a Tabbed Form<br>
        <a href="WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&ReadOnly=false">
          WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&ReadOnly=false</a></p>
    </form>

  </body>
</html>
