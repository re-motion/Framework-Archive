<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditPersonForm.aspx.cs" Inherits="WebSample.UI.EditPersonForm" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="rubicon" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Register TagPrefix="rubicon" TagName="EditPersonControl" Src="EditPersonControl.ascx" %>
<%@ Register Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" TagPrefix="dow" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>

<head runat="server">
  <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
</head>

<body>
  <form id="TheForm" runat="server">
    <dow:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="DomainSample.Person, DomainSample" />
    <rubicon:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView">
      <TopControls>
        <rubicon:NavigationTabs ID="TheNavigationTabs" runat="server" />
      </TopControls>
      <Views>
        <rubicon:TabView ID="EditPersonView" Title="$res:Details" runat="server">
          <rubicon:EditPersonControl ID="EditPersonControl" runat="server" />
        </rubicon:TabView>
      </Views>
      <BottomControls>
        <rubicon:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" />
        <rubicon:SmartLabel runat="server" Text="&nbsp;" />
        <rubicon:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" OnClick="CancelButton_Click" />
      </BottomControls>
    </rubicon:TabbedMultiView>
  </form>
</body>

</html>
