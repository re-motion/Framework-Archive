﻿<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="WebButtonTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.WebButtonTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>WebButton</h3>
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <remotion:WebButton ID="MyWebButton1Sync" Text="SyncButton" CommandName="Sync" RequiresSynchronousPostBack="true"  runat="server" />
      <remotion:WebButton ID="MyWebButton2Async" Text="AsyncButton" CommandName="Async" runat="server" />
      <div id="scope">
        <%-- ReSharper disable once Html.PathError --%>
        <remotion:WebButton ID="MyWebButton3Href" Text="HrefButton" PostBackUrl="WebButtonTest.wxe" runat="server" />
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>