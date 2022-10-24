using System;

using NUnit.Framework;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    /**
     * X.690 test example
     */
    [TestFixture]
    public class GeneralizedTimeTest
        : SimpleTest
    {
        private static readonly string[] input =
        {
            "20020122122220",
            "20020122122220Z",
            "20020122122220-1000",
            "20020122122220+00",
            "20020122122220.1",
            "20020122122220.1Z",
            "20020122122220.1-1000",
            "20020122122220.1+00",
            "20020122122220.01",
            "20020122122220.01Z",
            "20020122122220.01-1000",
            "20020122122220.01+00",
            "20020122122220.001",
            "20020122122220.001Z",
            "20020122122220.001-1000",
            "20020122122220.001+00",
            "20020122122220.0001",
            "20020122122220.0001Z",
            "20020122122220.0001-1000",
            "20020122122220.0001+00",
            "20020122122220.0001+1000"
        };

        private static readonly string[] output =
        {
            "20020122122220",
            "20020122122220GMT+00:00",
            "20020122122220GMT-10:00",
            "20020122122220GMT+00:00",
            "20020122122220.1",
            "20020122122220.1GMT+00:00",
            "20020122122220.1GMT-10:00",
            "20020122122220.1GMT+00:00",
            "20020122122220.01",
            "20020122122220.01GMT+00:00",
            "20020122122220.01GMT-10:00",
            "20020122122220.01GMT+00:00",
            "20020122122220.001",
            "20020122122220.001GMT+00:00",
            "20020122122220.001GMT-10:00",
            "20020122122220.001GMT+00:00",
            "20020122122220.0001",
            "20020122122220.0001GMT+00:00",
            "20020122122220.0001GMT-10:00",
            "20020122122220.0001GMT+00:00",
            "20020122122220.0001GMT+10:00"
        };

        private static readonly string[] zOutput =
        {
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122022220Z"
        };
        
        private static readonly string[] mzOutput =
        {
            "20020122122220.000Z",
            "20020122122220.000Z",
            "20020122222220.000Z",
            "20020122122220.000Z",
            "20020122122220.100Z",
            "20020122122220.100Z",
            "20020122222220.100Z",
            "20020122122220.100Z",
            "20020122122220.010Z",
            "20020122122220.010Z",
            "20020122222220.010Z",
            "20020122122220.010Z",
            "20020122122220.001Z",
            "20020122122220.001Z",
            "20020122222220.001Z",
            "20020122122220.001Z",
            "20020122122220.000Z",
            "20020122122220.000Z",
            "20020122222220.000Z",
            "20020122122220.000Z",
            "20020122022220.000Z"
        };

        private static readonly string[] derMzOutput =
        {
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122122220.1Z",
            "20020122122220.1Z",
            "20020122222220.1Z",
            "20020122122220.1Z",
            "20020122122220.01Z",
            "20020122122220.01Z",
            "20020122222220.01Z",
            "20020122122220.01Z",
            "20020122122220.001Z",
            "20020122122220.001Z",
            "20020122222220.001Z",
            "20020122122220.001Z",
            "20020122122220Z",
            "20020122122220Z",
            "20020122222220Z",
            "20020122122220Z",
            "20020122022220Z"
        };

        private static readonly string[] truncOutput =
        {
            "200201221222Z",
            "2002012212Z"
        };

        private static readonly string[] derTruncOutput =
        {
            "20020122122200Z",
            "20020122120000Z"
        };

        public override string Name
        {
            get { return "GeneralizedTime"; }
        }

        public override void PerformTest()
        {
            for (int i = 0; i != input.Length; i++)
            {
                string ii = input[i], oi = output[i];

                Asn1GeneralizedTime t = new Asn1GeneralizedTime(ii);
                DateTime dt = t.ToDateTime();
                string st = t.GetTime();

                if (oi.IndexOf('G') > 0)   // don't check local time the same way
                {
                    if (!st.Equals(oi))
                    {
                        Fail("failed GMT conversion test " + i);
                    }

                    string dts = dt.ToString(@"yyyyMMddHHmmss\Z");
                    string zi = zOutput[i];
                    if (!dts.Equals(zi))
                    {
                        Fail("failed date conversion test " + i);
                    }
                }
                else
                {
                    string offset = CalculateGmtOffset(dt);
                    if (!st.Equals(oi + offset))
                    {
                        Fail("failed conversion test " + i);
                    }
                }
            }

            for (int i = 0; i != input.Length; i++)
            {
                Asn1GeneralizedTime t = new Asn1GeneralizedTime(input[i]);

                if (!t.ToDateTime().ToString(@"yyyyMMddHHmmss.fff\Z").Equals(mzOutput[i]))
                {
                    Console.WriteLine("{0} != {1}", t.ToDateTime().ToString(@"yyyyMMddHHmmss.fff\Z"), mzOutput[i]);

                    Fail("failed long date conversion test " + i);
                }
            }

            for (int i = 0; i != mzOutput.Length; i++)
            {
                DerGeneralizedTime t = new DerGeneralizedTime(mzOutput[i]);

                if (!AreEqual(t.GetEncoded(), new Asn1GeneralizedTime(derMzOutput[i]).GetEncoded()))
                {
                    Fail("der encoding wrong");
                }
            }

            for (int i = 0; i != truncOutput.Length; i++)
            {
                DerGeneralizedTime t = new DerGeneralizedTime(truncOutput[i]);

                if (!AreEqual(t.GetEncoded(), new Asn1GeneralizedTime(derTruncOutput[i]).GetEncoded()))
                {
                    Fail("trunc der encoding wrong");
                }
            }

            {
                // check BER encoding is still "as given"
                Asn1GeneralizedTime ber = new Asn1GeneralizedTime("202208091215Z");

                //IsTrue(Arrays.AreEqual(Hex.Decode("180d3230323230383039313231355a"), ber.GetEncoded(Asn1Encodable.DL)));
                IsTrue(Arrays.AreEqual(Hex.Decode("180d3230323230383039313231355a"), ber.GetEncoded(Asn1Encodable.Ber)));
                IsTrue(Arrays.AreEqual(Hex.Decode("180f32303232303830393132313530305a"), ber.GetEncoded(Asn1Encodable.Der)));

                // check always uses DER encoding
                DerGeneralizedTime der = new DerGeneralizedTime("202208091215Z");

                //IsTrue(Arrays.AreEqual(Hex.Decode("180f32303232303830393132313530305a"), der.GetEncoded(Asn1Encodable.DL)));
                IsTrue(Arrays.AreEqual(Hex.Decode("180f32303232303830393132313530305a"), der.GetEncoded(Asn1Encodable.Ber)));
                IsTrue(Arrays.AreEqual(Hex.Decode("180f32303232303830393132313530305a"), der.GetEncoded(Asn1Encodable.Der)));
            }

            {
                // check an actual GMT string comes back untampered
                Asn1GeneralizedTime time = new Asn1GeneralizedTime("20190704031318GMT+00:00");

                IsTrue("20190704031318GMT+00:00".Equals(time.GetTime()));
            }

            try
            {
                new DerGeneralizedTime(new byte[0]);
            }
            catch (ArgumentException e)
            {
                IsTrue(e.Message.StartsWith("GeneralizedTime string too short"));
            }

            /*
             * [BMA-87]
             */
            {
                DateTime t1 = new DerUtcTime("110616114855Z").ToDateTime();
                DateTime t2 = new DerGeneralizedTime("20110616114855Z").ToDateTime();

                if (t1 != t2)
                {
                    Fail("failed UTC equivalence test");
                }

                DateTime u1 = t1.ToUniversalTime();
                DateTime u2 = t2.ToUniversalTime();

                if (u1 != u2)
                {
                    Fail("failed UTC conversion test");
                }
            }
        }

        private string CalculateGmtOffset(DateTime date)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.Local;
            TimeSpan offset = timeZone.BaseUtcOffset;

            char sign = '+';
            if (offset.CompareTo(TimeSpan.Zero) < 0)
            {
                sign = '-';
                offset = offset.Duration();
            }

            int hours = offset.Hours;
            int minutes = offset.Minutes;

            if (timeZone.SupportsDaylightSavingTime && timeZone.IsDaylightSavingTime(date))
            {
                hours += sign.Equals("+") ? 1 : -1;
            }

            return "GMT" + sign + Convert(hours) + ":" + Convert(minutes);
        }

        private string Convert(int time)
        {
            if (time < 10)
            {
                return "0" + time;
            }

            return time.ToString();
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
