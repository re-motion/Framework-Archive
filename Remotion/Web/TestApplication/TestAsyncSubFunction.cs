using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace TestApplication
{
  [Serializable]
    public class TestAsyncSubFunction : AsyncFunction
    {
      public TestAsyncSubFunction (params object[] actualParameters)
          : base(new NoneTransactionMode(), actualParameters)
      {
      }

      public TestAsyncSubFunction (WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
          : base(new NoneTransactionMode(), parameterDeclarations, actualParameters)
      {
      }

      protected override async Task BeginExecute ()
        {
            Debug.WriteLine(">>> sub function starting!!");

            await PageStep("SubFunction.aspx");

            Debug.WriteLine (">>> sub function completed!!");
        }
    }
}