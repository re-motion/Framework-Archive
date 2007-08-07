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
    <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
    <div>
      <rubicon:WebButton ID="PostBackButton" runat="server" Text="Post Back" />
      <rubicon:WebButton ID="SaveButton" runat="server" Width="120px" Text="Save" />
      <rubicon:WebButton ID="SaveAndRestartButton" runat="server" Text="Save &amp; Restart" Width="120px" />
      <rubicon:WebButton ID="CancelButton" runat="server" Text="Cancel" Width="120px" />
    </div>
    <div>
      <asp:UpdatePanel ID="UserControlUpdatePanel" runat="server">
        <contenttemplate>
          <asp:PlaceHolder ID="UserControlPlaceHolder" runat="server" />
        </contenttemplate>
      </asp:UpdatePanel>
    </div>
    <div>
      <asp:UpdatePanel ID="StackUpdatePanel" runat="server">
        <contenttemplate>
          <asp:Literal ID="Stack" runat="server" />
          <asp:Button runat="server" ID="stackbutton"/>
        </contenttemplate>
      </asp:UpdatePanel>
    </div>
    <rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.ObjectBinding.Sample::Person" />
  </form>
</body>
</html>
