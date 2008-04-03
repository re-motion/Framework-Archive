<%@ Register TagPrefix="Template" Namespace="Remotion.Templates.Generic.Web.Classes" Assembly="Remotion.Templates.Generic.Web" %>
<%@ Register TagPrefix="Remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="Remotion.Templates.Generic.Web.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
      BackColor="White" StatusMessage="###"><Remotion:Tab id="TemplateTab" Label="Template Form"><Remotion:TabMenu id="TemplateMenu" Label="" href="WxeHandler.ashx?WxeFunctionType=Remotion.Templates.Generic.Web.WxeFunctions.TemplateFunction,Remotion.Templates.Generic.Web"></Remotion:TabMenu></Remotion:Tab><Remotion:Tab id="OtherTemplateTab" Label="Other Template Form"><Remotion:TabMenu id="OtherTemplateMenu" Label="" href="WxeHandler.ashx?WxeFunctionType=Remotion.Templates.Generic.Web.WxeFunctions.OtherTemplateFunction,Remotion.Templates.Generic.Web"></Remotion:TabMenu></Remotion:Tab>
  </Template:WxeTabControl>
</div>
