using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestApplication
{
  public class TestAsyncFunction : AsyncFunction
  {


    protected override async Task BeginExecute()
    {
      Debug.WriteLine(">>> function starting!!");

      await PageStep("Step2.aspx");

      //await Step21();

      await PageStep("Step3.aspx");

      Debug.WriteLine(">>> function completed!!");
    }

    //public async Task Step21()
    //{
    //  await PageStep("Step3.aspx");
    //}
  }
}