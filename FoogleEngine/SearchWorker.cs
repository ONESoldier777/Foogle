using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class SearchWorker
    {
        public class SearchMatch
        {
            public Page Page;
            public string SearchPhrase;
            public double MatchPercentage;
            public List<string> Words;
            public List<int> Hits;

            public SearchMatch(Page page, string searchPharse)
            {
                this.Page = page;
                SearchPhrase = searchPharse.ToLower();
                Words = SearchPhrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Hits = new List<int>();
                foreach (string word in Words)
                {
                    Hits.Add(Regex.Matches(Page.SearchableText, word).Count);
                }
                MatchPercentage = ((double)Hits.Where(h => h > 0).Count()) / ((double)Hits.Count);
            }

            public List<string> SearchMatchText()
            {
                List<string> result = new List<string>();
                result.Add("<a href=\"" + Page.Url.ToString() + "\">" + Page.Title + "</a>");
                result.Add(MatchPercentage.ToString("P") + " matches from phrase.");
                result.Add(Hits.Sum() + " combined hits.");
                return result;
            }
        }
        public FatabaseWorker fb;

        public SearchWorker(string connection)
        {
            fb = new FoogleEngine.FatabaseWorker(connection);
        }

        public IEnumerable<SearchMatch> Search(string searchText)
        {
            IEnumerator<DataTable> data = fb.RetrievePages().GetEnumerator();

            while (data.MoveNext())
            {
                if (data.Current != null)
                {
                    foreach (DataRow dr in data.Current.Rows)
                    {
                        if (((bool)dr["Parsable"]))
                        {
                            Page page = new FoogleEngine.Page(dr["Html"].ToString(), new Uri(dr["Url"].ToString()));
                            SearchMatch sm = new FoogleEngine.SearchWorker.SearchMatch(page, searchText);
                            if (sm.MatchPercentage > 0)
                                yield return sm;
                        }
                    }
                }
            }
        }
    }
}
