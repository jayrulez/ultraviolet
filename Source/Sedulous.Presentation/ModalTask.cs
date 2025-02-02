﻿using System;
using System.Threading.Tasks;
using Sedulous.Core;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents a task which completes when the modal dialog that spawned the task is closed.
    /// </summary>
    public class ModalTask : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalTask"/> class which wraps the specified task.
        /// </summary>
        /// <param name="task">The task which is wrapped by this instance.</param>
        internal ModalTask(Task task)
        {
            Contract.Require(task, nameof(task));

            this.task = task;
        }

        /// <summary>
        /// Releases resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            task.Dispose();
        }

        /// <summary>
        /// Creates a continuation which executes when the task is complete.
        /// </summary>
        /// <param name="action">An action to run when the task completes.</param>
        /// <returns>A new continuation task.</returns>
        public ModalTask ContinueWith(Action<Task> action)
        {
            return new ModalTask(task.ContinueWith(action, TaskContinuationOptions.ExecuteSynchronously));
        }

        /// <summary>
        /// Gets the task's status.
        /// </summary>
        public TaskStatus Status
        {
            get { return task.Status; }
        }

        /// <summary>
        /// Gets a value indicating whether the task has been canceled.
        /// </summary>
        public Boolean IsCanceled
        {
            get { return task.IsCanceled; }
        }

        /// <summary>
        /// Gets a value indicating whether the task has been completed.
        /// </summary>
        public Boolean IsCompleted
        {
            get { return task.IsCompleted; }
        }

        /// <summary>
        /// Gets a value indicating whether the task has been faulted.
        /// </summary>
        public Boolean IsFaulted
        {
            get { return task.IsFaulted; }
        }

        // State values.
        private readonly Task task;
    }
}
