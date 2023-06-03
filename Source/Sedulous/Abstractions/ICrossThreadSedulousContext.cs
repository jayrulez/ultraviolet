using System;
using System.Threading;
using System.Threading.Tasks;
using Sedulous.Core;
using Sedulous.Core.Messages;

namespace Sedulous
{
    /// <summary>
    /// Represents an interface to an <see cref="SedulousContext"/> which only exposes
    /// members which are safe to call from a background thread.
    /// </summary>
    public interface ICrossThreadSedulousContext
    {
        /// <inheritdoc cref="SedulousContext.SpawnTask(Action{CancellationToken})" />
        Task SpawnTask(Action<CancellationToken> action);

        /// <inheritdoc cref="SedulousContext.QueueWorkItem(Action{object}, object, WorkItemOptions)" />
        Task QueueWorkItem(Action<Object> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="SedulousContext.QueueWorkItem(Func{object, Task}, object, WorkItemOptions)" />
        Task QueueWorkItem(Func<Object, Task> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="SedulousContext.QueueWorkItem{T}(Func{object, T}, object, WorkItemOptions)" />
        Task<T> QueueWorkItem<T>(Func<Object, T> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="SedulousContext.QueueWorkItem{T}(Func{object, Task{T}}, object, WorkItemOptions)" />
        Task<T> QueueWorkItem<T>(Func<Object, Task<T>> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None);

        /// <inheritdoc cref="SedulousContext.Runtime" />
        SedulousRuntime Runtime { get; }

        /// <inheritdoc cref="SedulousContext.Platform" />
        SedulousPlatform Platform { get; }

        /// <inheritdoc cref="SedulousContext.Messages" />
        IMessageQueue<SedulousMessageID> Messages { get; }
    }
}
