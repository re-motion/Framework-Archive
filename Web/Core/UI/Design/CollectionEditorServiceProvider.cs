using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

namespace Rubicon.Web.UI.Design
{

public class CollectionEditorServiceProvider: IServiceProvider, IWindowsFormsEditorService
{
  private IServiceProvider _baseServiceProvider = null;
  private Size _editorSize;
  private double _propertyGridLabelRatio;

  public CollectionEditorServiceProvider (
      IServiceProvider baseServiceProvider,
      Size editorSize, 
      double propertyGridLabelRatio)
  {
    _baseServiceProvider = baseServiceProvider;
    _editorSize = editorSize;
    _propertyGridLabelRatio = propertyGridLabelRatio;
  }

  public CollectionEditorServiceProvider (
      IServiceProvider baseServiceProvider,
      int editorWidth, 
      int editorHeight, 
      double propertyGridLabelRatio)
    : this (baseServiceProvider, new Size (editorWidth, editorHeight), propertyGridLabelRatio)
	{
  }

	public CollectionEditorServiceProvider (IServiceProvider baseServiceProvider)
    : this (baseServiceProvider, new Size (300, 400), 2)
	{
  }

  public virtual object GetService (Type serviceType)
  {
    if (serviceType == typeof (System.Windows.Forms.Design.IWindowsFormsEditorService))
      return this;
    else
      return _baseServiceProvider.GetService (serviceType);
  }

  public void DropDownControl(Control control)
  {
    throw new NotSupportedException();
  }

  public void CloseDropDown()
  {
    throw new NotSupportedException();
  }

  public DialogResult ShowDialog (Form dialog)
  {
    dialog.Size = _editorSize;
    dialog.StartPosition = FormStartPosition.CenterParent;

    PropertyGrid propertyGrid = GetPropertyGrid (dialog);
    SetPropertyGridSplitter (propertyGrid, _propertyGridLabelRatio);
    propertyGrid.HelpVisible = true;
    propertyGrid.BackColor = SystemColors.Control;
    
    return dialog.ShowDialog();
  }

  private PropertyGrid GetPropertyGrid (Form editor)
  {
    const string collectionEditorCollectionFormTypeName = "System.ComponentModel.Design.CollectionEditor+CollectionEditorCollectionForm";

    BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

    Type collectionEditorCollectionFormType = editor.GetType();
    if (collectionEditorCollectionFormType.FullName != collectionEditorCollectionFormTypeName)
    {
      throw new ArgumentException (
          string.Format ("Argument {0} has type {2} when type {1} was expected.",
          "editor",
          collectionEditorCollectionFormTypeName,
          collectionEditorCollectionFormType), "editor");
    }

    //  HACK: CollectionEditorServiceProvider: Reflection on private PropertyGrid CollectionEditorCollectionForm.propertyBrowser
    //  private PropertyGrid System.ComponentModel.Design.CollectionEditor+CollectionEditorCollectionForm.propertyBrowser
    FieldInfo propertyBrowserFieldInfo = collectionEditorCollectionFormType.GetField (
        "propertyBrowser",
        bindingFlags);
    PropertyGrid propertyBrowser = (PropertyGrid) propertyBrowserFieldInfo.GetValue(editor);

    return propertyBrowser;
  }

  private void SetPropertyGridSplitter (PropertyGrid propertyGrid, double labelRatio)
  {
    BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

    //  class PropertyGridView is internal

    //  HACK: CollectionEditorServiceProvider: Reflection on private PropertyGridView PropertyGrid.GetPropertyGridView()
    //  private PropertyGridView System.Windows.Forms.PropertyGrid.GetPropertyGridView()
    MethodInfo getPropertyGridViewMethodInfo = typeof (PropertyGrid).GetMethod (
        "GetPropertyGridView",
        bindingFlags);
    object propertyGridView = getPropertyGridViewMethodInfo.Invoke (propertyGrid, null);

    //  HACK: CollectionEditorServiceProvider: Reflection on public double PropertyGridView.labelRatio
    //  public double System.Windows.Forms.PropertyGridInternal.GetPropertyGridView.labelRatio
    Type propertyGridViewType = propertyGridView.GetType();
    FieldInfo labelRatioFieldInfo = propertyGridViewType.GetField ("labelRatio");
    labelRatioFieldInfo.SetValue(propertyGridView, labelRatio);
  }

  public Size EditorSize
  {
    get { return _editorSize; }
    set { _editorSize = value; }
  }

  public double PropertyGridLabelRatio
  {
    get { return _propertyGridLabelRatio; }
    set { _propertyGridLabelRatio = value; }
  }
}

}
