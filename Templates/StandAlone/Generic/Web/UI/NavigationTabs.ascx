<%@ Register TagPrefix="Template" Namespace="Rubicon.Templates.Generic.Web.Classes" Assembly="Rubicon.Templates.Generic.Web" %>
<%@ Register TagPrefix="Rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="Rubicon.Templates.Generic.Web.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div id="PageHeader">
  <table cellpadding="0" cellspacing="0" width="100%" border="0">
    <tr>
      <td width="330"><img src="Image/rublogo.gif" alt="RUBICON"></td>
      <td align="right">
        <div id="ApplicationInfo">Template Application</div>
      </td>
    </tr>
  </table>
</div>
<div id="NavigationTab">
  <Template:WxeTabControl id="NavigationTabControl" runat="server" Height="30" ActiveTab="0" SeperatorLine="true"
      LineColor="white" ServerSideNavigation="true" FirstImage="Image/tableft.gif" SecondImage="Image/tabright.gif"
      EmptyImage="Image/ws.gif" InactiveClass="inactiveTab" ActiveClass="activeTab" HasMenuBar="true" ActiveColor="Blue"
      BackColor="White" StatusMessage="###"><Rubicon:Tab id="TemplateTab" Label="Template Form"><Rubicon:TabMenu id="TemplateMenu" Label="" href="WxeHandler.ashx?WxeFunctionType=Rubicon.Templates.Generic.Web.WxeFunctions.TemplateFunction,Rubicon.Templates.Generic.Web"></Rubicon:TabMenu></Rubicon:Tab><Rubicon:Tab id="OtherTemplateTab" Label="Other Template Form"><Rubicon:TabMenu id="OtherTemplateMenu" Label="" href="WxeHandler.ashx?WxeFunctionType=Rubicon.Templates.Generic.Web.WxeFunctions.OtherTemplateFunction,Rubicon.Templates.Generic.Web"></Rubicon:TabMenu></Rubicon:Tab>
  </Template:WxeTabControl>
</div>
