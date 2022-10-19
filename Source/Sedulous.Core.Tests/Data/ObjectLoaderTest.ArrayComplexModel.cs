using System;
using Sedulous.Core.Data;

namespace Sedulous.Core.Tests.Data
{
    /// <summary>
    /// Represents a model used by the object loader tests to validate loading of arrays of complex objects.
    /// </summary>
    public class ObjectLoader_ArrayComplexModel : DataObject
    {
        public ObjectLoader_ArrayComplexModel(String key, Guid globalID)
            : base(key, globalID)
        {

        }

        public readonly ObjectLoader_ComplexRefObject[] ArrayValue;
    }
}
