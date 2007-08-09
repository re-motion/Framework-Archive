<%@ Page Language="c#" Codebehind="Form.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.IndividualControlTests.IndividualControlTestForm" %>

<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
<head>
  <title>IndividualControlTestForm</title>
  <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Rubicon.ObjectBinding.Sample::Person" />
    <rubicon:SingleView ID="SingleView" runat="server">
      <TopControls>
        <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
        <asp:PlaceHolder ID="ButtonPlaceHolder" runat="server">
          <div>
            <rubicon:WebButton ID="PostBackButton" runat="server" Text="Post Back" />
            <rubicon:WebButton ID="SaveButton" runat="server" Width="120px" Text="Save" />
            <rubicon:WebButton ID="SaveAndRestartButton" runat="server" Text="Save &amp; Restart" Width="120px" />
            <rubicon:WebButton ID="CancelButton" runat="server" Text="Cancel" Width="120px" />
          </div>
        </asp:PlaceHolder>
      </TopControls>
      
      <ViewTemplate>
        <asp:UpdatePanel ID="UserControlUpdatePanel" runat="server">
          <contenttemplate>
            <asp:PlaceHolder ID="UserControlPlaceHolder" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
      </ViewTemplate>
      
      <BottomControls>
      </BottomControls>
    </rubicon:SingleView>
        <asp:UpdatePanel ID="StackUpdatePanel" runat="server">
          <contenttemplate>
            <asp:Literal ID="Stack" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
  </form>
</body>
</html>
