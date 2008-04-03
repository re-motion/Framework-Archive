using System;
using System.ComponentModel;
using System.Web.UI;
using Remotion.ObjectBinding.Design;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Design
{

[Obsolete ("Only required when configuring a component in the VS.NET ASP.NET Designer.")]
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
