using System;
using Sedulous.Core.Data;

namespace Sedulous.Core.Tests.Data
{
    /// <summary>
    /// Represents a simple model used by the object loader tests.
    /// </summary>
    public abstract class ObjectLoader_SimpleModelBase : DataObject
    {
        public ObjectLoader_SimpleModelBase(String key, Guid globalID)
            : base(key, 
            globalID)
        {

        }

        public readonly String StringValueOnBaseClass;
    }
}
