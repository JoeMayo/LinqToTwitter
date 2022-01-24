using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LinqToTwitter.OAuth
{
    internal class OAuthListener
    {
        public string Listen(string ip, int port)
        {
            TcpListener? server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse(ip), port);
                server.Start();

                using TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();

                int readCount;
                var bytes = new byte[256];
                var sb = new StringBuilder();

                while ((readCount = stream.Read(bytes, 0, bytes.Length)) != 0)
                    sb.Append(Encoding.UTF8.GetString(bytes, 0, readCount));

                return sb.ToString();
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}
