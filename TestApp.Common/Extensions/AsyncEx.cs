using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.Common.Extensions
{
    public static class AsyncEx
    {
        public static async Task<T> Retry<T>(this Func<Task<T>> task, int retries, TimeSpan delay,
            CancellationToken cts = default) =>
            await task()
                .ContinueWith(async innerTask =>
                {
                    cts.ThrowIfCancellationRequested();

                    if (TaskStatus.Faulted != innerTask.Status)
                        return innerTask.Result;

                    if (0 == retries)
                        throw innerTask.Exception ?? throw new Exception();

                    await Task.Delay(delay, cts);

                    return await Retry(task, retries - 1, delay, cts);
                }, cts)
                .Unwrap();

        public static Task<T> Otherwise<T>(this Task<T> task, Func<Task<T>> otherTask) =>
            task
                .ContinueWith(async innerTask =>
                {
                    if (TaskStatus.Faulted == innerTask.Status)
                        return await otherTask();

                    return innerTask.Result;
                })
                .Unwrap();
    }
}