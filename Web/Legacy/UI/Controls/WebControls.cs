using System;

using Microsoft.Web.UI.WebControls;

namespace Rubicon.Findit.Client.Controls
{

public class TabStrip : Microsoft.Web.UI.WebControls.TabStrip
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

}

public class MultiPage : Microsoft.Web.UI.WebControls.MultiPage
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

}
  
public class PageView : Microsoft.Web.UI.WebControls.PageView
{
  protected override RenderPathID RenderPath
  {
    get 
    { 
    	if (this.IsDesignMode)
		    return RenderPathID.DesignerPath;
      else
	  	  return RenderPathID.DownLevelPath;
    }
  }

}
}
