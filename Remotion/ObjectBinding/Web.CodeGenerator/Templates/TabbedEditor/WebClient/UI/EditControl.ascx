<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit$DOMAIN_CLASSNAME$Control.ascx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Control" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="$DOMAIN_QUALIFIEDCLASSTYPENAME$" />

<div>
  <table id="$DOMAIN_CLASSNAME$FormGrid" runat="server">
  $REPEAT_FOREACHPROPERTY_BEGIN$
  <tr>
    <td><remotion:SmartLabel ID="$DOMAIN_PROPERTYNAME$Label" runat="server" ForControl="$DOMAIN_PROPERTYNAME$Field" /></td>
    <td>
      <$CONTROLTYPE$ ID="$DOMAIN_PROPERTYNAME$Field" runat="server" DataSourceControl="CurrentObject"
          PropertyIdentifier="$DOMAIN_PROPERTYNAME$" $ADDITIONALATTRIBUTES$>
        $ADDITIONALELEMENTS$
      </$CONTROLTYPE$>
    </td>
  </tr>$REPEAT_FOREACHPROPERTY_END$
</table>

</div>
