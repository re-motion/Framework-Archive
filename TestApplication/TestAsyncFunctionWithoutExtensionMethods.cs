using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestApplication
{
  public class TestAsyncFunctionWithoutExtensionMethods : AsyncFunction
  {
    protected override async Task BeginExecute()
    {
      await PageStep("Step2.aspx");

      await Step2_1();

      var result = await Step2_2();  

      //2
      Console.WriteLine ("result: " + result);

      await PageStep("Step3.aspx");
    }

    public async Task Step2_1()
    {
      await PageStep("Step21.aspx");
    }

    public async Task<int> Step2_2()
    {
      await PageStep("Step22.aspx"); 

      //1
      return await Step2_2_1(); 
    }

    public async Task<int> Step2_2_1()
    {
      await PageStep("Step221.aspx");

      //3
      return 123;
    }
  }
}