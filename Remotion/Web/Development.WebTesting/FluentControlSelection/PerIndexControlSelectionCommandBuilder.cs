﻿using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerIndexControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerIndexControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerIndexControlSelectionCommandBuilder<TControlSelector, TControlObject> : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerIndexControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly int _index;

    public PerIndexControlSelectionCommandBuilder (int index)
    {
      _index = index;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      
      return new PerIndexControlSelectionCommand<TControlObject> (controlSelector, _index);
    }
  }
}