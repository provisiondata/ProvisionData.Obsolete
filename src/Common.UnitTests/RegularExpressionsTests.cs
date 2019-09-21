using Shouldly;
using Xunit;

namespace ProvisionData
{
    public class RegularExpressionsTests
    {
        [Fact]
        public void PortNumber()
        {
            Ptn.PortNumber.IsMatch("0").ShouldBe(true);
            Ptn.PortNumber.IsMatch("1").ShouldBe(true);
            Ptn.PortNumber.IsMatch("9").ShouldBe(true);
            Ptn.PortNumber.IsMatch("10").ShouldBe(true);
            Ptn.PortNumber.IsMatch("99").ShouldBe(true);
            Ptn.PortNumber.IsMatch("100").ShouldBe(true);
            Ptn.PortNumber.IsMatch("999").ShouldBe(true);
            Ptn.PortNumber.IsMatch("1000").ShouldBe(true);
            Ptn.PortNumber.IsMatch("9999").ShouldBe(true);
        }

        [Fact]
        public void IPv4()
        {
            Ptn.IPv4.IsMatch("0.0.0.0").ShouldBe(true);
            Ptn.IPv4.IsMatch("00.00.00.00").ShouldBe(true);
            Ptn.IPv4.IsMatch("000.000.000.000").ShouldBe(true);
            Ptn.IPv4.IsMatch("1.1.1.1").ShouldBe(true);
            Ptn.IPv4.IsMatch("01.01.01.01").ShouldBe(true);
            Ptn.IPv4.IsMatch("001.001.001.001").ShouldBe(true);

            // Netmask
            Ptn.IPv4.IsMatch("255.255.255.255").ShouldBe(true);

            // Private networks
            Ptn.IPv4.IsMatch("10.10.10.10").ShouldBe(true);
            Ptn.IPv4.IsMatch("172.16.0.0").ShouldBe(true);
            Ptn.IPv4.IsMatch("192.168.0.0").ShouldBe(true);

            // Bad IPs - First Octet
            Ptn.IPv4.IsMatch("355.255.255.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("265.255.255.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("256.255.255.255").ShouldBe(false);

            // Bad IPs - Second Octet
            Ptn.IPv4.IsMatch("255.355.255.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.265.255.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.256.255.255").ShouldBe(false);

            // Bad IPs - Third Octet
            Ptn.IPv4.IsMatch("255.255.355.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.255.265.255").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.255.256.255").ShouldBe(false);

            // Bad IPs - Fourth Octet
            Ptn.IPv4.IsMatch("255.255.255.355").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.255.255.265").ShouldBe(false);
            Ptn.IPv4.IsMatch("255.255.255.256").ShouldBe(false);
        }

        [Fact]
        public void IPv4AndPort()
        {
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:0").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:1").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:10").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:100").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:1000").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:10000").ShouldBe(true);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:65535").ShouldBe(true);

            Ptn.IPv4AndPort.IsMatch("10.10.10.10:65536").ShouldBe(false);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:65545").ShouldBe(false);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:65635").ShouldBe(false);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:66535").ShouldBe(false);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:75535").ShouldBe(false);
            Ptn.IPv4AndPort.IsMatch("10.10.10.10:100000").ShouldBe(false);
        }

        [Fact]
        public void IPv4AndOptionalPort()
        {
            Ptn.IPv4AndOptionalPort.IsMatch("0.0.0.0").ShouldBe(true);
            Ptn.IPv4AndOptionalPort.IsMatch("10.10.10.10").ShouldBe(true);
            Ptn.IPv4AndOptionalPort.IsMatch("255.255.255.255").ShouldBe(true);

            Ptn.IPv4AndOptionalPort.IsMatch("10.10.10.10:0").ShouldBe(true);
            Ptn.IPv4AndOptionalPort.IsMatch("10.10.10.10:1").ShouldBe(true);
            Ptn.IPv4AndOptionalPort.IsMatch("10.10.10.10:65535").ShouldBe(true);

            Ptn.IPv4AndOptionalPort.IsMatch("256.255.255.255").ShouldBe(false);
            Ptn.IPv4AndOptionalPort.IsMatch("255.255.255.255:65536").ShouldBe(false);
        }
    }
}
