<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" 
  Inherits="Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.EditObjectPage" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="rubicon" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Bestehendes Objekt mit allen Datentypen editieren</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <rwc:htmlheadcontents id="Htmlheadcontents1" runat="server"></rwc:htmlheadcontents>
  </HEAD>
  <body>
    <form id="DefaultForm" method="post" runat="server">
      <rubicon:ControlWithAllDataTypes ID="ControlWithAllDataTypesControl" runat="server" />
    </form>
  </body>
</HTML>
