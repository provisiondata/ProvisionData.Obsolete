namespace ProvisionData.GELF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public enum Protocol
    {
        UDP = 0,
        TCP = 1,
        HTTP = 2,
        HTTPS = 3
    }
}