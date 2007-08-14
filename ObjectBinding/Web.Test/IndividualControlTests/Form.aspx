<%@ Page Language="c#" Codebehind="Form.aspx.cs" AutoEventWireup="True" Inherits="OBWTest.IndividualControlTests.IndividualControlTestForm" %>

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
            <rubicon:WebButton ID="PostBackButton" runat="server" Text="Post Back"/>&nbsp;
            <rubicon:WebButton ID="SaveButton" runat="server" Width="10em" Text="Save" />&nbsp;
            <rubicon:WebButton ID="SaveAndRestartButton" runat="server" Width="10em" Text="Save &amp; Restart" />&nbsp;
            <rubicon:WebButton ID="CancelButton" runat="server" Width="10em" Text="Cancel" />
          </div>
        </asp:PlaceHolder>
      </TopControls>
      
      <View>
        <asp:UpdatePanel ID="UserControlUpdatePanel" runat="server">
          <contenttemplate>
            <asp:PlaceHolder ID="UserControlPlaceHolder" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
      </View>
      
      <BottomControls>
         <asp:UpdatePanel ID="StackUpdatePanel" runat="server" UpdateMode="Conditional">
          <contenttemplate>
            <asp:Literal ID="Stack" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
     </BottomControls>
    </rubicon:SingleView>
  </form>
</body>
</html>
