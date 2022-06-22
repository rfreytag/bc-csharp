#if !LIB
using System;

using NUnit.Core;
using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.IO.Tests
{
	public class AllTests
	{
        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("IO tests");
                suite.Add(new CipherStreamTest());
                return suite;
            }
        }
	}
}
#endif
