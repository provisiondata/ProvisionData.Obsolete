using Shouldly;
using Xunit;

namespace ProvisionData
{
    public class RegularExpressionsTests
    {

        [Fact]
        public void PortNumber()
        {
            RegularExpressions.PortNumber.IsMatch("0").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("1").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("9").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("10").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("99").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("100").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("999").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("1000").ShouldBe(true);
            RegularExpressions.PortNumber.IsMatch("9999").ShouldBe(true);
        }

        [Fact]
        public void IPv4()
        {
            RegularExpressions.IPv4.IsMatch("0.0.0.0").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("00.00.00.00").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("000.000.000.000").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("1.1.1.1").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("01.01.01.01").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("001.001.001.001").ShouldBe(true);

            // Netmask
            RegularExpressions.IPv4.IsMatch("255.255.255.255").ShouldBe(true);

            // Private networks
            RegularExpressions.IPv4.IsMatch("10.10.10.10").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("172.16.0.0").ShouldBe(true);
            RegularExpressions.IPv4.IsMatch("192.168.0.0").ShouldBe(true);

            // Bad IPs - First Octet
            RegularExpressions.IPv4.IsMatch("355.255.255.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("265.255.255.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("256.255.255.255").ShouldBe(false);

            // Bad IPs - Second Octet
            RegularExpressions.IPv4.IsMatch("255.355.255.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.265.255.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.256.255.255").ShouldBe(false);

            // Bad IPs - Third Octet
            RegularExpressions.IPv4.IsMatch("255.255.355.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.255.265.255").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.255.256.255").ShouldBe(false);

            // Bad IPs - Fourth Octet
            RegularExpressions.IPv4.IsMatch("255.255.255.355").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.255.255.265").ShouldBe(false);
            RegularExpressions.IPv4.IsMatch("255.255.255.256").ShouldBe(false);
        }

        [Fact]
        public void IPv4AndPort()
        {
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:0").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:1").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:10").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:100").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:1000").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:10000").ShouldBe(true);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:65535").ShouldBe(true);


            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:65536").ShouldBe(false);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:65545").ShouldBe(false);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:65635").ShouldBe(false);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:66535").ShouldBe(false);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:75535").ShouldBe(false);
            RegularExpressions.IPv4AndPort.IsMatch("10.10.10.10:100000").ShouldBe(false);
        }

        [Fact]
        public void IPv4AndOptionalPort()
        {
            RegularExpressions.IPv4AndOptionalPort.IsMatch("0.0.0.0").ShouldBe(true);
            RegularExpressions.IPv4AndOptionalPort.IsMatch("10.10.10.10").ShouldBe(true);
            RegularExpressions.IPv4AndOptionalPort.IsMatch("255.255.255.255").ShouldBe(true);

            RegularExpressions.IPv4AndOptionalPort.IsMatch("10.10.10.10:0").ShouldBe(true);
            RegularExpressions.IPv4AndOptionalPort.IsMatch("10.10.10.10:1").ShouldBe(true);
            RegularExpressions.IPv4AndOptionalPort.IsMatch("10.10.10.10:65535").ShouldBe(true);

            RegularExpressions.IPv4AndOptionalPort.IsMatch("256.255.255.255").ShouldBe(false);
            RegularExpressions.IPv4AndOptionalPort.IsMatch("255.255.255.255:65536").ShouldBe(false);
        }
    }
}
