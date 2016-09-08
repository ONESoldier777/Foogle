using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoogleEngine
{
    public class Page
    {
        public List<string> PageLinks = new List<string>();

        public Uri Url;
        public string Html;
        public string Title;
        public string SearchableText;
        internal Stack<PageElement> ElementsStack = new Stack<PageElement>();
        PageParser pageParser;

        string createSearchableText()
        {
            // This is called by the Property SearcjableText and is a method that will be trashed
            var html = Children.Where(x => x as PageElement != null && ((PageElement)x).ElementName == "html").FirstOrDefault();
            if (html != null)
            {
                var body = ((PageElement)html).Children.Where(x => x as PageElement != null && ((PageElement)x).ElementName == "body").FirstOrDefault();
                if (body != null)
                {
                    return recursiveGetTextualObjects((PageElement)body, new StringBuilder());
                }
            }
            return "";
        }

        string recursiveGetTextualObjects(PageElement currentObject, StringBuilder stringBuilder)
        {
            foreach (var child in currentObject.Children)
            {
                PageElement tempPE = child as PageElement;
                if (tempPE != null)
                {
                    if (tempPE.ElementName == "script")
                        continue;
                    recursiveGetTextualObjects(tempPE, stringBuilder);
                }
                else
                {
                    stringBuilder.Append(child.html);
                }
            }
            return stringBuilder.ToString();
        }

        public Page(string pageString, Uri url)
        {
            Url = url;
            Html = pageString;
            pageParser = new FoogleEngine.PageParser(this);
            pageParser.Parse(pageString);
            
        }

        public List<PageObject> Children = new List<PageObject>();
    }
}