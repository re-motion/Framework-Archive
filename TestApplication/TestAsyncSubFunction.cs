using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestApplication
{
    public class TestAsyncSubFunction : AsyncFunction
    {
        protected override async Task BeginExecute ()
        {
            Debug.WriteLine(">>> sub function starting!!");

            await PageStep("SubFunction.aspx");

            Debug.WriteLine (">>> sub function completed!!");
        }
    }
}