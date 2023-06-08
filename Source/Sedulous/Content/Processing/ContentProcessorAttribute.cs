using System;

namespace Sedulous.Content
{
    /// <summary>
    /// Represents an attribute which marks a class as a content processor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ContentProcessorAttribute : Attribute
    {

    }
}
