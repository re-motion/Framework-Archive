<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.DefaultPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>default</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body >
    <form id="Form1" method="post" runat="server">
      
      <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.Data.DomainObjects.Web.Test.WxeFunctions.EditObjectFunction, Rubicon.Data.DomainObjects.Web.Test">Bestehendes Objekt editieren</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.Data.DomainObjects.Web.Test.WxeFunctions.NewObjectFunction, Rubicon.Data.DomainObjects.Web.Test">Neues Objekt editieren</a><br>
      <a href="SearchObject.aspx" target="_blank">Objekt suchen</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.Data.DomainObjects.Web.Test.WxeFunctions.WxeTestPageFunction,Rubicon.Data.DomainObjects.Web.Test&amp;ClassWithAllDataTypesID=ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid" target="_blank">WxeFunctionTest</a> 
    </form>
  </body>
</HTML>
