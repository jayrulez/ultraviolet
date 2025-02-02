﻿using System;
using System.Reflection;
using Sedulous.Core;

namespace Sedulous.Presentation.Uvml
{
    /// <summary>
    /// Represents a UVML mutator which sets a standard property value.
    /// </summary>
    internal sealed class UvmlStandardPropertyValueMutator : UvmlMutator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UvmlStandardPropertyValueMutator"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property which is being mutated.</param>
        /// <param name="propertyValue">The value to set on the property.</param>
        public UvmlStandardPropertyValueMutator(PropertyInfo propertyInfo, UvmlNode propertyValue)
        {
            Contract.Require(propertyInfo, nameof(propertyInfo));
            Contract.Require(propertyValue, nameof(propertyValue));

            this.propertyInfo = propertyInfo;
            this.propertyValue = propertyValue;
        }

        /// <inheritdoc/>
        public override Object InstantiateValue(FrameworkContext frameworkContext, Object instance, UvmlInstantiationContext context)
        {
            return propertyValue.Instantiate(frameworkContext, context);
        }

        /// <inheritdoc/>
        public override void Mutate(FrameworkContext frameworkContext, Object instance, UvmlInstantiationContext context)
        {
            var value = InstantiateValue(frameworkContext, instance, context);
            Mutate(frameworkContext, instance, value, context);
        }
        
        /// <inheritdoc/>
        public override void Mutate(FrameworkContext frameworkContext, Object instance, Object value, UvmlInstantiationContext context)
        {
            var processedValue = ProcessPrecomputedValue<Object>(value, context);
            propertyInfo.SetValue(instance, processedValue, null);
        }

        // State values.
        private readonly PropertyInfo propertyInfo;
        private readonly UvmlNode propertyValue;
    }
}
