using System;

namespace FoogleEngine
{
    public class IndexedPage
    {
        public Page Page;

        internal static IndexedPage Create(string pageString, string url)
        {
            return new IndexedPage() { Page = new Page(pageString, new Uri(url)) };
        }
    }
}