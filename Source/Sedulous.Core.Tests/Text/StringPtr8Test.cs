using System;
using NUnit.Framework;
using Sedulous.Core.TestFramework;
using Sedulous.Core.Text;

namespace Sedulous.Core.Tests.Text
{
    [TestFixture]
    public class StringPtr8Test : CoreTestFramework
    {
        [Test]
        public void StringPtr8_HandlesNullTerminatedString()
        {
            unsafe
            {
                var data = new byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0 };
                fixed (byte* pdata = data)
                {
                    var ptr = new StringPtr8((IntPtr)pdata);

                    TheResultingString(ptr.ToString())
                        .ShouldBe("Hello");
                }
            }
        }
    }
}
