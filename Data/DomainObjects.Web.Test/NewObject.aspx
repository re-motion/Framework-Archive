<%@ Page language="c#" Codebehind="NewObject.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.NewObjectPage" %>
<%@ Register TagPrefix="rubicon" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Neues Objekt mit allen Datentypen editieren</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <rubicon:htmlheadcontents id="Htmlheadcontents1" runat="server"></rubicon:htmlheadcontents>
  </HEAD>
  <body>
    <form id="DefaultForm" method="post" runat="server">
      <rubicon:ControlWithAllDataTypes ID="ControlWithAllDataTypesControl" runat="server" />
    </form>
  </body>
</HTML>
