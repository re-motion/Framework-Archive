<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="WebUserControl1.ascx.cs" Inherits="OBWTest.WebUserControl1" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<obw:BocTextValueValidator id="BocTextValueValidator1" runat="server"></obw:BocTextValueValidator>
<obw:BocTextValue id="BocTextValue1" runat="server" DataSource="<%# reflectionBusinessObjectDataSource2 %>">
</obw:BocTextValue>
