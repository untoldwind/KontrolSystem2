using System;
using System.Threading;
using System.Threading.Tasks;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    [KSModule("core::background",
        Description = "Provides means to run functions as asynchronous background task."
    )]
    public class CoreBackground {
        [KSClass("Task", Description = "Represents a background task")]
        public class BackgroundTask<T> {
            private readonly Task<T> task;
            private readonly CancellationTokenSource tokenSource;

            internal BackgroundTask(Task<T> task, CancellationTokenSource tokenSource) {
                this.task = task;
                this.tokenSource = tokenSource;
            }

            [KSField(Description = "Check if the task is completed")]
            public bool IsCompleted => task.IsCompleted;

            [KSField(Description = "Check if the task is completed and has a value")]
            public bool IsSuccess => task.IsCompleted && !task.IsCanceled && !task.IsFaulted;

            [KSField(Description = "Check if the task has been canceled")]
            public bool IsCanceled => task.IsCanceled;

            [KSField(Description = "Get the result of the task once completed")]
            public T Result => task.Result;

            [KSMethod(Description = "Cancel/abort the task")]
            public void Cancel() => tokenSource.Cancel();
        }

        [KSFunction(Description = "Check if current thread is a background thread")]
        public static bool IsBackground() => ContextHolder.CurrentContext.Value?.IsBackground ?? false;

        [KSFunction(Description = "Run a function as background task.")]
        public static BackgroundTask<T> Run<T>(Func<T> function) {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            IContext backgroundContext = ContextHolder.CurrentContext.Value.CloneBackground(tokenSource);
            Task<T> task = Task.Run(() => {
                ContextHolder.CurrentContext.Value = backgroundContext;
                T result = function();
                ContextHolder.CurrentContext.Value = null;

                return result;
            }, tokenSource.Token);

            return new BackgroundTask<T>(task, tokenSource);
        }
    }
}
