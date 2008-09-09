using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles <see cref="Array"/> instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderArrayHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;

      if (type.IsArray)
      {
        toTextBuilder.WriteArray ((Array) obj);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}