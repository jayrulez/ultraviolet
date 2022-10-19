using System;
using Sedulous.Core.Data;

namespace Sedulous.Core.Tests.Data
{
    partial class DataObjectRegistryTests
    {
        public class TestDataObjectRegistry : DataObjectRegistry<TestDataObject>
        {
            public override String DataElementName => "TestDataObject";
            public override String ReferenceResolutionName => "test";
        }
    }
}
