using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Data.DomainObjects.ObjectBinding.Design;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web
{
public class DomainObjectDataSourceControl : BusinessObjectDataSourceControl
{
  public static object GetDesignTimePropertyValue (Control control, string propertyName)
  {
    if (! ControlHelper.IsDesignMode (control))
      return null;

    try
    {
      ISite site = control.Site;

      //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
      MethodInfo getServiceMethod = site.GetType().GetMethod ("GetService");
      Type _DTEType = Type.GetType ("EnvDTE._DTE, EnvDTE");
      object[] arguments = new object[] { _DTEType };
      object environment = getServiceMethod.Invoke (site, arguments);

      if (environment != null)
      {
        //EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;
        object activeDocument =_DTEType.InvokeMember ("ActiveDocument", BindingFlags.GetProperty, null, environment, null);
        object projectItem = activeDocument.GetType().InvokeMember ("ProjectItem", BindingFlags.GetProperty, null, activeDocument, null);
        object project = projectItem.GetType().InvokeMember ("ContainingProject", BindingFlags.GetProperty, null, projectItem, null);

        ////project.Properties uses a 1-based index
        //foreach (EnvDTE.Property property in project.Properties)
        object properties = project.GetType().InvokeMember ("Properties", BindingFlags.GetProperty, null, project, null);
        IEnumerator propertiesEnumerator = (IEnumerator) properties.GetType().InvokeMember ("GetEnumerator", BindingFlags.InvokeMethod, null, properties, null);
        while (propertiesEnumerator.MoveNext())
        {
          object property = propertiesEnumerator.Current;

          //if (property.Name == propertyName)
          string projectPropertyName = (string) property.GetType().InvokeMember ("Name", BindingFlags.GetProperty, null, property, null);
          if (projectPropertyName == propertyName)
          {
            //return property.Value;
            return property.GetType().InvokeMember ("Value", BindingFlags.GetProperty, null, property, null);
          }
        }
      }
    }
    catch
    {
      return null;
    }

    return null;
  }

  private DomainObjectDataSource _dataSource = new DomainObjectDataSource();

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    SetDesignTimeMapping ();
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue("")]
  [Editor (typeof (ClassPickerEditor), typeof (UITypeEditor))]
  public string TypeName
  {
    get 
    { 
      return _dataSource.TypeName; 
    }
    set
    { 
      _dataSource.TypeName = value; 
    }
  }

  protected override IBusinessObjectDataSource GetDataSource()
  {
    return _dataSource;
  }

  private void SetDesignTimeMapping ()
  {
    string projectPath = (string) GetDesignTimePropertyValue (this, "ActiveFileSharePath");
    if (projectPath == null)
      return;

    MappingLoader mappingLoader = new MappingLoader (projectPath + @"\bin\mapping.xml", projectPath + @"\bin\mapping.xsd");
    MappingConfiguration.SetCurrent (new MappingConfiguration (mappingLoader));
  }
}
}
