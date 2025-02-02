﻿using System;
using System.Reflection;
using Sedulous.Core;

namespace Sedulous.Presentation.Uvml
{
    /// <summary>
    /// Represents a UVML mutator which sets a dependency property value.
    /// </summary>
    internal sealed class UvmlDependencyPropertyValueMutator : UvmlMutator
    {
        /// <summary>
        /// Initializes the <see cref="UvmlDependencyPropertyValueMutator"/> type.
        /// </summary>
        static UvmlDependencyPropertyValueMutator()
        {
            var dobjMethods = typeof(DependencyObject).GetMethods();

            foreach (var dobjMethod in dobjMethods)
            {
                if (miSetLocalValue != null)
                    break;

                if (miSetLocalValue == null && String.Equals(dobjMethod.Name, nameof(DependencyObject.SetLocalValue), StringComparison.Ordinal))
                {
                    if (dobjMethod.GetParameters()[0].ParameterType == typeof(DependencyProperty))
                    {
                        miSetLocalValue = dobjMethod;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UvmlDependencyPropertyValueMutator"/> class.
        /// </summary>
        /// <param name="dpropID">The property which is being mutated.</param>
        /// <param name="dpropValue">The value to set on the property.</param>
        public UvmlDependencyPropertyValueMutator(DependencyProperty dpropID, UvmlNode dpropValue)
        {
            Contract.Require(dpropID, nameof(dpropID));
            Contract.Require(dpropValue, nameof(dpropValue));

            this.dpropID = dpropID;
            this.dpropValue = dpropValue;
            this.dpropSetter = miSetLocalValue.MakeGenericMethod(dpropID.PropertyType);
        }

        /// <inheritdoc/>
        public override Object InstantiateValue(FrameworkContext frameworkContext, Object instance, UvmlInstantiationContext context)
        {
            return dpropValue.Instantiate(frameworkContext, context);
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
            var dobj = instance as DependencyObject;
            if (dobj == null)
                return;

            var processedValue = ProcessPrecomputedValue<Object>(value, context);
            dpropSetter.Invoke(instance, new Object[] { dpropID, processedValue });
        }

        // Reflection information for the open generic version of SetLocalValue() on DependencyObject
        private static readonly MethodInfo miSetLocalValue;

        // State values.
        private readonly DependencyProperty dpropID;
        private readonly UvmlNode dpropValue;
        private readonly MethodInfo dpropSetter;
    }
}
