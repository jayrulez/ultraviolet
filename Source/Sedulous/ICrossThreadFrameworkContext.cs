using System;
using System.Threading;
using System.Threading.Tasks;
using Sedulous.Core;
using Sedulous.Core.Messages;

namespace Sedulous
{
    /// <summary>
    /// Represents an interface to an <see cref="FrameworkContext"/> which only exposes
    /// members which are safe to call from a background thread.
    /// </summary>
    public interface ICrossThreadFrameworkContext
    {
        /// <inheritdoc cref="FrameworkContext.SpawnTask(Action{CancellationToken})" />
        Task SpawnTask(Action<CancellationToken> action);

        /// <inheritdoc cref="FrameworkContext.QueueWorkItem(Action{object}, object, WorkItemOptions)" />
        Task QueueWorkItem(Action<Object> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="FrameworkContext.QueueWorkItem(Func{object, Task}, object, WorkItemOptions)" />
        Task QueueWorkItem(Func<Object, Task> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="FrameworkContext.QueueWorkItem{T}(Func{object, T}, object, WorkItemOptions)" />
        Task<T> QueueWorkItem<T>(Func<Object, T> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="FrameworkContext.QueueWorkItem{T}(Func{object, Task{T}}, object, WorkItemOptions)" />
        Task<T> QueueWorkItem<T>(Func<Object, Task<T>> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="FrameworkContext.Runtime" />
        FrameworkRuntime Runtime { get; }

        /// <inheritdoc cref="FrameworkContext.Platform" />
        FrameworkPlatform Platform { get; }

        /// <inheritdoc cref="FrameworkContext.Messages" />
        IMessageQueue<FrameworkMessageID> Messages { get; }
    }
}
