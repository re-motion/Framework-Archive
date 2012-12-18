using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CustomSynchronizationContext
{
  internal class Program
  {
    public static async Task Startup (BroadcastBlock<string> broadcastBlock)
    {
      await broadcastBlock.SendAsync ("begin Startup()");
      int res = await Sub1 (broadcastBlock);
      await broadcastBlock.SendAsync ("Sub1() returned " + res);
      await broadcastBlock.SendAsync ("end Startup()");
    }

    public static async Task<int> Sub1 (BroadcastBlock<string> broadcastBlock)
    {
      await broadcastBlock.SendAsync ("Sub1");
      return 1;
    }

    public static void Main ()
    {
      var singleThradedTaskScheduler = new SingleThreadedTaskScheduler();

      var broadcastBlock = new BroadcastBlock<string> (
          s => s,
          new DataflowBlockOptions
          {
              TaskScheduler = singleThradedTaskScheduler,
          });

      var actionBlock = new ActionBlock<string> (
          s => Console.WriteLine (s),
          new ExecutionDataflowBlockOptions { TaskScheduler = singleThradedTaskScheduler });
      broadcastBlock.LinkTo (actionBlock, new DataflowLinkOptions ());

      var task = new Task (() => Startup (broadcastBlock));
      task.RunSynchronously (singleThradedTaskScheduler);
    }
  }

  public class SingleThreadedTaskScheduler : TaskScheduler
  {
    protected override void QueueTask (Task task)
    {
      base.TryExecuteTask (task);
    }

    protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
    {
      return base.TryExecuteTask (task);
    }

    protected override IEnumerable<Task> GetScheduledTasks ()
    {
      throw new NotImplementedException();
    }
  }
}