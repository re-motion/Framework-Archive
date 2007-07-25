<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EmptyUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.EmptyUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="obrt" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>

<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<rubicon:formgridmanager id=FormGridManager runat="server"/><rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" typename="OBRTest.Person, OBRTest"/></div>
