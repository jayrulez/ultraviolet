using System;
using System.Threading;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents an identifier for a message send through the Sedulous context's message queue. 
    /// </summary>
    public partial struct SedulousMessageID : IEquatable<SedulousMessageID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousMessageID"/> structure.
        /// </summary>
        /// <param name="name">The message type's name.</param>
        /// <param name="value">The message's identifier value.</param>
        private SedulousMessageID(String name, Int32 value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Acquires an unused message identifier.
        /// </summary>
        /// <param name="name">The message type's name.</param>
        /// <returns>The <see cref="SedulousMessageID"/> that was acquired.</returns>
        public static SedulousMessageID Acquire(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            var id = Interlocked.Increment(ref counter);
            return new SedulousMessageID(name, id);
        }

        /// <inheritdoc/>
        public override String ToString() => name ?? "INVALID";

        /// <summary>
        /// Gets the message identifier's underlying value.
        /// </summary>
        public Int32 Value
        {
            get { return value; }
        }

        // The message identifier's underlying value.
        private static Int32 counter;
        private Int32 value;
        private String name;
    }
}
