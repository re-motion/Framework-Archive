using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Utilities;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Design
{

public class ComponentBindingExpressionUtility
{
  public static IComponent ResolveExpression (IControl control, string expression)
  {
    return ResolveExpression ((Control)control, expression);
  }

  public static IComponent ResolveExpression (Control control, string expression)
  {
    expression = expression.Trim();

    IResolveComponentBindingExpression resolveComponentBindingExpression = ControlHelper.GetParentTemplateControl (control) as IResolveComponentBindingExpression;
    if (resolveComponentBindingExpression != null)
      return resolveComponentBindingExpression.Resolve (expression);

    if (control.Site != null && control.Site.Container != null)
      return control.Site.Container.Components [expression];
    else
      return null;
  }
}

}
