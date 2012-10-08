using System.Collections;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  public interface IRowIDProvider
  {
    string GetControlRowID (BocListRow row);
    string GetItemRowID (BocListRow row);
    BocListRow GetRowFromItemRowID (IList rows, string rowID);
  }
}