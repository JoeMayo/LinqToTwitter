using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LinqToTwitter.OAuth
{
    internal class OAuthListener
    {
        const string DefaultContents = @"
    <h1>Success!</h1>
    <p>You've successfully authenticated your app.</p>
    <p>
      Follow <a href=""https://twitter.com/JoeMayo"">@JoeMayo</a> for the latest updates.
    </p>
";
        const string HtmlResponseTemplate = @"
HTTP/1.1 200 OK

<html>
  <body>
    <div>
        {0}
    </div>
    <p>
      <b>Powered By:</b> <a href=""https://github.com/JoeMayo/LinqToTwitter"">LINQ to Twitter!</a>
    </p>
  </body>
</html>
";

        readonly string responseHtml;

        public OAuthListener(string? responseHtml = null)
        {
            string contents = string.IsNullOrWhiteSpace(responseHtml) ? DefaultContents : responseHtml;

            this.responseHtml = string.Format(HtmlResponseTemplate, contents);
        }

        /// <summary>
        /// Listens for an authorization token response from Twitter
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Listen(string ip, int port)
        {
            TcpListener? server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse(ip), port);
                server.Start();

                using TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();

                if (stream.CanTimeout)
                {
                    stream.ReadTimeout = 500;
                    stream.WriteTimeout = 500;
                }

                int readCount;
                var bytes = new byte[1024];
                var sb = new StringBuilder();

                while ((readCount = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    sb.Append(Encoding.UTF8.GetString(bytes, 0, readCount));

                    if (readCount < bytes.Length)
                        break;
                }

                stream.Write(Encoding.UTF8.GetBytes(responseHtml));

                return sb.ToString();
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}
