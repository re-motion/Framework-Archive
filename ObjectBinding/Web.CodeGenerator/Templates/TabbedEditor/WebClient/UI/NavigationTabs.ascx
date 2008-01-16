<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationTabs.ascx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.NavigationTabs" %>

<rubicon:TabbedMenu ID="TheTabbedMenu" runat="server">
  <Tabs>
    $REPEAT_FOREACHCLASS_BEGIN$
    <rubicon:MainMenuTab ItemID="$DOMAIN_CLASSNAME$Tab" Text="$res:$DOMAIN_CLASSNAME$">
      <PersistedCommand>
        <rubicon:NavigationCommand type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <rubicon:SubMenuTab ItemID="Edit$DOMAIN_CLASSNAME$Tab" Text="$res:New">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="Edit$DOMAIN_CLASSNAME$" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
        <rubicon:SubMenuTab ItemID="Search$DOMAIN_CLASSNAME$Tab" Text="$res:List">
          <PersistedCommand>
            <rubicon:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="Search$DOMAIN_CLASSNAME$" />
          </PersistedCommand>
        </rubicon:SubMenuTab>
      </SubMenuTabs>
    </rubicon:MainMenuTab>
    $REPEAT_FOREACHCLASS_END$
  </Tabs>
</rubicon:TabbedMenu>
