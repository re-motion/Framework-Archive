using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;

using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A read only collection of <see cref="FormGridManager.FormGridRow"/> objects. </summary>
public sealed class FormGridRowInfoCollection : CollectionBase
{
  /// <summary> 
  ///   Constructor allowing the initalization of the collection with an array of
  ///   <see cref="FormGridRowInfo"/> objects.
  /// </summary>
  /// <param name="prototypes">
  ///  The array of <see cref="FormGridRowInfo"/> object to be managed by this collection.
  ///  May not contain <see langword="null"/>.
  /// </param>
  public FormGridRowInfoCollection (FormGridRowInfo[] prototypes)
  {
    ArgumentUtility.CheckNotNull ("prototypes", prototypes);

    for (int index = 0; index < prototypes.Length; index++)
    {
      if (prototypes[index] == null)
        throw new ArgumentNullException ("prototypes[" + index + "]");
    }

    InnerList.AddRange (prototypes);
  }

  /// <summary> Simple Constructor. </summary>
  public FormGridRowInfoCollection()
  {}

  /// <summary> Allows only the insertion of form grid row prototypes. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (FormGridRowInfo));
    base.OnInsert (index, value);
  }

  /// <summary> Adds the form grid row prototype to the end of the list. </summary>
  /// <param name="prototype">. The new form grid row prototype. </param>
  public void Add (FormGridRowInfo prototype)
  {
    ArgumentUtility.CheckNotNull ("prototype", prototype);

    InnerList.Add (prototype);
  }
}

}
