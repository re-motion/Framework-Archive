using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestApplication
{
  public class TestAsyncFunction : AsyncFunction
  {


    protected override async Task BeginExecute()
    {
      await PageStep("Step2.aspx");

      await Step21().ConfigureAwait(ExecutionIterator);

      var result = await Step22().ConfigureAwait (ExecutionIterator);

      Console.WriteLine ("result: " + result);

      await PageStep("Step3.aspx");
    }

    public async Task Step21()
    {
      await PageStep("Step21.aspx");
    }

    public async Task<int> Step22()
    {
      await PageStep("Step22.aspx");
      return 123;
    }
  }
}