using System;

namespace Sedulous
{
    partial struct FrameworkMessageID
    {
        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            return value.GetHashCode();
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are equal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator ==(FrameworkMessageID v1, FrameworkMessageID v2)
        {
            return v1.Equals(v2);
        }
        
        /// <summary>
        /// Compares two objects to determine whether they are unequal.
        /// </summary>
        /// <param name="v1">The first value to compare.</param>
        /// <param name="v2">The second value to compare.</param>
        /// <returns><see langword="true"/> if the two values are unequal; otherwise, <see langword="false"/>.</returns>
        public static Boolean operator !=(FrameworkMessageID v1, FrameworkMessageID v2)
        {
            return !v1.Equals(v2);
        }
        
        /// <inheritdoc/>
        public override Boolean Equals(Object other)
        {
            return (other is FrameworkMessageID x) ? Equals(x) : false;
        }
        
        /// <inheritdoc/>
        public Boolean Equals(FrameworkMessageID other)
        {
            return
                this.value == other.value;
        }
    }
}
