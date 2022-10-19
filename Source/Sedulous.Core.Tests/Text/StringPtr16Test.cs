using System;
using NUnit.Framework;
using Sedulous.Core.TestFramework;
using Sedulous.Core.Text;

namespace Sedulous.Core.Tests.Text
{
    [TestFixture]
    public class StringPtr16Test : CoreTestFramework
    {
        [Test]
        public void StringPtr16_HandlesNullTerminatedString()
        {
            unsafe
            {
                var data = new ushort[] { 'H', 'e', 'l', 'l', 'o', 0 };
                fixed (ushort* pdata = data)
                {
                    var ptr = new StringPtr16((IntPtr)pdata);
                    
                    TheResultingString(ptr.ToString())
                        .ShouldBe("Hello");
                }
            }
        }
    }
}
