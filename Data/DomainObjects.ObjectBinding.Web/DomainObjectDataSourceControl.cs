using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding.Design;
using Remotion.Data.DomainObjects.ObjectBinding.Web.Design;
using Remotion.Design;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;
using Remotion.Web.Utilities;
using Remotion.Data.DomainObjects.ObjectBinding.Infrastructure;

namespace Remotion.Data.DomainObjects.ObjectBinding.Web
{
  [Designer (typeof (DomainObjectDataSourceDesigner))]
  public class DomainObjectDataSourceControl: BusinessObjectDataSourceControl
  {
    private DomainObjectDataSource _dataSource = new DomainObjectDataSource();
    private Exception _designTimeMappingException = null;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _dataSource.Site = Site;
      SetDesignTimeMapping();
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue ("")]
    [Editor (typeof (ClassPickerEditor), typeof (UITypeEditor))]
    public string TypeName
    {
      get { return _dataSource.TypeName; }
      set { _dataSource.TypeName = value; }
    }

    protected override IBusinessObjectDataSource GetDataSource()
    {
      return _dataSource;
    }

    private void SetDesignTimeMapping()
    {
      if (ControlHelper.IsDesignMode (this, Context))
      {
        try
        {
          DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (new WebDesginModeHelper (Site));
          domainObjectsDesignModeHelper.InitializeConfiguration();
        }
        catch (Exception e)
        {
          _designTimeMappingException = e;
        }
      }
    }

    internal Exception GetDesignTimeException()
    {
      return _designTimeMappingException;
    }
  }
}