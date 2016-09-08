using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Foogle
{
    public class foogleHub : Hub
    {
        public static FoogleEngine.FatabaseWorker fb = new FoogleEngine.FatabaseWorker(ConfigurationManager.ConnectionStrings["FoogleFatabase"].ConnectionString);
        public static FoogleEngine.SearchWorker sw = new FoogleEngine.SearchWorker(ConfigurationManager.ConnectionStrings["FoogleFatabase"].ConnectionString);
        public const int LevelsDeep = 3;
        public void Hello()
        {
            Clients.All.hello();
        }

        public override Task OnConnected()
        {
            Groups.Add(Context.ConnectionId, Context.ConnectionId);
            return base.OnConnected();
        }
        public void IndexPage(string url)
        {
            List<string> messages = new List<string>();
            messages.Add("Foogle is beginning indexing...");
            IndexMessageAll(messages);

            FoogleEngine.Page page;
            List<string> parsedLinks = new List<string>();
            // http://stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp
            if (FoogleEngine.IndexWorker.TryPageParse(url, out page))
            {
                parsedLinks.Add(page.Url.ToString());
                int linksCount = 1;
                int linksParsed = 1;
                recursiveIndexLinks(messages, page, parsedLinks, ref linksCount, ref linksParsed);
            } else
            {
                messages.Clear();
                messages.Add("Could not determine the proper encoding of page, please try another url.");
                IndexMessageAll(messages);
            }
        }

        void IndexMessageAll(List<string> messages)
        {
            Clients.All.clearIndexMessages();
            foreach (var message in messages)
            {
                Clients.All.sendIndexMessage(message);
            }
        }
        void recursiveIndexLinks(List<string> messages, FoogleEngine.Page page, List<string> parsedLinks, ref int linksCount, ref int linksParsed, int level = 2)
        {
            linksCount += page.PageLinks.Count;
            foreach (string link in page.PageLinks)
            {
                int index = parsedLinks.BinarySearch(link);
                if (index < 0)
                {
                    linksParsed++;
                    FoogleEngine.Page childPage;
                    if (FoogleEngine.IndexWorker.TryPageParse(link, out childPage))
                    {
                        parsedLinks.Insert(Math.Abs(index) - 1, childPage.Url.ToString());
                    }
                    if (level < LevelsDeep && childPage != null)
                    {
                        recursiveIndexLinks(messages, childPage, parsedLinks, ref linksCount, ref linksParsed, level + 1);
                    }

                    fb.IndexPage(childPage, link);
                }
                messages.Clear();
                messages.Add("Foogle is beginning indexing...");
                messages.Add("Foogle has currently found (" + linksCount + ") connections to Index");
                messages.Add("Foogle has indexed, " + linksParsed + " of " + linksCount + ".");
                IndexMessageAll(messages);
            }
        }

        void SearchMessageGroup(List<string> messages)
        {
            Clients.Group(Context.ConnectionId).clearSearchMessages();
            foreach (var message in messages)
            {
                Clients.Group(Context.ConnectionId).sendSearchMessage(message);
            }
        }
        string FunnyDots(int index)
        {
            List<string> dots = new List<string>()
            {
                ".....",
                "o....",
                ".o...",
                "..o..",
                "...o.",
                "....o"
            };
            return dots[index];
        }
        public void SearchInterwebs(string searchText)
        {
            List<string> messages = new List<string>();
            messages.Add("Searching for matches....");
            SearchMessageGroup(messages);
            List<FoogleEngine.SearchWorker.SearchMatch> matches = new List<FoogleEngine.SearchWorker.SearchMatch>();
            foreach (FoogleEngine.SearchWorker.SearchMatch match in sw.Search(searchText))
            {
                if (match.MatchPercentage > 0)
                {
                    matches.Add(match);
                    messages.Clear();
                    messages.Add("Searching for matches....<br /><br />");
                    foreach (var m in matches)
                    {
                        messages.AddRange(m.SearchMatchText());
                        messages.Add("<br /><br />");
                    }
                    SearchMessageGroup(messages);
                }
            }
            messages.Clear();
            foreach (var m in matches)
            {
                messages.AddRange(m.SearchMatchText());
                messages.Add("<br /><br />");
            }
            SearchMessageGroup(messages);
        }
    }
}