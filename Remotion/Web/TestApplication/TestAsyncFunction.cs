using System;
using System.Threading.Tasks;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace TestApplication
{
  [Serializable]
  public class TestAsyncFunction : AsyncFunction
  {
    public TestAsyncFunction ()
        : base(new NoneTransactionMode())
    {
    }

    public TestAsyncFunction (params object[] actualParameters)
        : base(new NoneTransactionMode(), actualParameters)
    {
    }

    public TestAsyncFunction (WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
        : base(new NoneTransactionMode(), parameterDeclarations, actualParameters)
    {
    }

    protected override async Task BeginExecute()
    {
      await PageStep("Step2.aspx");
      await Step2_1().ConfigureAwait(ExecutionIterator);
      var result = await Step2_2().ConfigureAwait (ExecutionIterator);  
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
      return await Step2_2_1().ConfigureAwait (ExecutionIterator); 
    }

    public async Task<int> Step2_2_1()
    {
      await PageStep("Step221.aspx");
      //3
      return 123;
    }
  }
}