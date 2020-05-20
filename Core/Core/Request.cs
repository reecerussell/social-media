using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Core
{
    public static class Request
    {
        public static T ParseBody<T>(this APIGatewayProxyRequest request)
        {
            var json = request.Body;

            if (request.IsBase64Encoded)
            {
                var data = Convert.FromBase64String(request.Body);
                json = Encoding.UTF8.GetString(data);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static (Stream Stream, string ContentType) ParseFile(this APIGatewayProxyRequest request, string formName)
        {
            var rawData = Convert.FromBase64String(request.Body);
            var content = Encoding.UTF8.GetString(rawData);

            var contentType = string.Empty;
            MemoryStream data = null;

            var delimiterEndIndex = content.IndexOf("\r\n", StringComparison.InvariantCulture);
            if (delimiterEndIndex > -1)
            {
                var delimiter = content.Substring(0, content.IndexOf("\r\n", StringComparison.InvariantCulture));
                var sections = content.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < sections.Length; i++)
                {
                    var s = sections[i];

                    if (!s.Contains("Content-Disposition"))
                    {
                        continue;
                    }

                    var nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                    var name = nameMatch.Value.Trim().ToLower();

                    if (name != formName)
                    {
                        continue;
                    }

                    var contentTypeMatch = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)").Match(content);
                    if (contentTypeMatch.Success)
                    {
                        contentType = contentTypeMatch.Value.Trim();

                        var startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                        var delimiterBytes = Encoding.UTF8.GetBytes("\r\n" + delimiter);
                        var endIndex = IndexOf(rawData, delimiterBytes, startIndex);
                        var contentLength = endIndex - startIndex;

                        var fileData = new byte[contentLength];
                        Buffer.BlockCopy(rawData, startIndex, fileData, 0, contentLength);
                        data = new MemoryStream(fileData);
                    }
                }
            }
            else
            {
                throw new ArgumentException("form data was in invalid format");
            }

            if (data == null)
            {
                throw new InvalidOperationException("no file data in request body");
            }

            return (data, contentType);
        }

        private static int IndexOf(byte[] searchWithin, byte[] searchFor, int startIndex)
        {
            var index = 0;
            var startPos = Array.IndexOf(searchWithin, searchFor[0], startIndex);

            if (startPos == -1)
            {
                return -1;
            }

            while ((startPos + index) < searchWithin.Length)
            {
                if (searchWithin[startPos + index] == searchFor[index])
                {
                    index++;
                    if (index == searchFor.Length)
                    {
                        return startPos;
                    }
                }
                else
                {
                    startPos = Array.IndexOf(searchWithin, searchFor[0], startPos + index);
                    if (startPos == -1)
                    {
                        return -1;
                    }
                    index = 0;
                }
            }

            return -1;
        }
    }
}
