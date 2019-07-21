using System.IO;
using System.Text;

namespace Netlenium.Intellivoid
{
    internal class HttpUrlEncodedRequestParser : HttpRequestParser
    {
        private readonly MemoryStream stream;

        public HttpUrlEncodedRequestParser(HttpClient client, int contentLength)
            : base(client, contentLength)
        {
            stream = new MemoryStream();
        }

        public override void Parse()
        {
            Client.ReadBuffer.CopyToStream(stream, ContentLength);

            if (stream.Length == ContentLength)
            {
                ParseContent();

                EndParsing();
            }
        }

        private void ParseContent()
        {
            stream.Position = 0;

            string content;

            using (var reader = new StreamReader(stream, Encoding.ASCII))
            {
                content = reader.ReadToEnd();
            }

            HttpUtil.UrlDecodeTo(content, Client.PostParameters);
        }
    }
}
