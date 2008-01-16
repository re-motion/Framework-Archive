<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit$DOMAIN_CLASSNAME$Form.aspx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Form" %>
<%@ Register TagPrefix="app" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Register TagPrefix="rubicon" TagName="Edit$DOMAIN_CLASSNAME$Control" Src="Edit$DOMAIN_CLASSNAME$Control.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
  <title>$res:$DOMAIN_CLASSNAME$</title>
  <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
</head>

<body>
  <form id="ThisForm" runat="server">
    <rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="$DOMAIN_QUALIFIEDCLASSTYPENAME$" />
    <rubicon:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView" >
      <TopControls>
        <app:NavigationTabs ID="TheNavigationTabs" runat="server" />
      </TopControls>
      <Views>
        <rubicon:TabView ID="Edit$DOMAIN_CLASSNAME$View" Title="$res:Details" runat="server">
          <rubicon:Edit$DOMAIN_CLASSNAME$Control ID="Edit$DOMAIN_CLASSNAME$Control" runat="server" />
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
