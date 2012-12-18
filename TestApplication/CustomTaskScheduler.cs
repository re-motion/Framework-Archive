using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestApplication
{
  public class CustomTaskScheduler : TaskScheduler
  {
    private readonly AsyncExecutionIterator _iterator;

    public CustomTaskScheduler (AsyncExecutionIterator iterator)
    {
      _iterator = iterator;
    }

    protected override void QueueTask (Task task)
    {
      _iterator.EnqueueContinuation (new Continuation (() => task.RunSynchronously (this), () => true));
    }

    protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
    {
      return base.TryExecuteTask (task);
    }

    protected override IEnumerable<Task> GetScheduledTasks ()
    {
      throw new System.NotImplementedException();
    }
  }
}