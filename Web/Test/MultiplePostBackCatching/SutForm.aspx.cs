using System;

namespace Rubicon.Web.Test.MultiplePostBackCatching
{
  public partial class SutForm : BasePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      SutGenerator.GenerateSut (this, SutPlaceHolder.Controls);
    }
  }
}