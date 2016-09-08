using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    static public class IndexWorker
    {
        static public bool TryPageParse(string url, out Page parsedPage)
        {
            parsedPage = null;
            try
            {
                bool foundEncoding = false;
                string pageString;
                using (WebClient client = new WebClient())
                {
                    byte[] data = client.DownloadData(url);
                    ContentType contentType = new System.Net.Mime.ContentType(client.ResponseHeaders[HttpResponseHeader.ContentType]);
                    pageString = Encoding.GetEncoding(contentType.CharSet).GetString(data);
                    if (pageString.Contains("html") == false)
                    {
                        foreach (var encoding in Encoding.GetEncodings())
                        {
                            client.Encoding = encoding.GetEncoding();
                            pageString = client.DownloadString(url);
                            if (pageString.Contains("html"))
                            {
                                foundEncoding = true;
                                break;
                            }
                        }
                    }
                    else
                        foundEncoding = true;
                }
                if (foundEncoding)
                    parsedPage = new FoogleEngine.Page(pageString, new Uri(url));
                return parsedPage != null;
            }
            catch (Exception ex)
            {
                // Swallow result and like it
                parsedPage = null;
                return false;
            }
        }
    }
}
