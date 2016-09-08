using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class PageElement : PageObject
    {
        public enum PageElementTagType
        {
            Opening,
            Closing,
            SelfClosed,
            Broken
        }

        public string ElementName;
        public PageElementTagType TagType;

        public PageElement Parent;
        public List<PageObject> Children = new List<PageObject>();

        public List<PageObject> Attributes = new List<PageObject>();
        List<PageObject> possibles = new List<PageObject>();
        List<PageObject> factories;

        bool hasLink = false;
        bool inValue = false;
        bool nextIsValue = false;

        public PageElement(Page page) : base(page)
        {
            this.StartingCharacterFilters.Add('<');
            this.EndingCharacterFilters.Add('>');

            factories = new List<PageObject>()
            {
                new ElementAttribute(Page),
                new ElementAttributeValue(Page),
                new WhitespaceObject(Page)
            };
        }

        public override void AddCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            html.Append(currentChar);

            if (currentChar == '=')
                nextIsValue = true;

            if (IsEndingCharacter(lastChar, currentChar, nextChar))
            {
                IsCompleted = true;
                string tempString = html.ToString();
                // First check if it is a closing element, if not check if it is a self closed, if not it is a opening element
                if (tempString[1] == '/')
                {
                    TagType = PageElementTagType.Closing;
                    ElementName = tempString.Substring(2, tempString.Length - 3).ToLower();
                }
                else if (tempString[tempString.Length - 2] == '/')
                {
                    TagType = PageElementTagType.SelfClosed;
                    ElementName = tempString.Substring(1, tempString.IndexOfAny(new char[] { ' ', '/' })).ToLower();
                }
                else
                {
                    TagType = PageElementTagType.Opening;
                    ElementName = tempString.Substring(1, tempString.IndexOfAny(new char[] { ' ', '/', '>' }) - 1).ToLower();
                }

                if (TagType == PageElementTagType.Opening)
                {
                    this.Page.ElementsStack.Push(this);
                }
                else if (TagType == PageElementTagType.Closing)
                {
                    PageElement current = this.Page.ElementsStack.Pop();
                    while (current.ElementName != this.ElementName)
                    {
                        PageElement tempElement = current;
                        tempElement.TagType = PageElementTagType.Broken;
                        current = this.Page.ElementsStack.Pop();
                        current.AddChildren(tempElement.Children);
                        tempElement.Children.Clear();
                    }
                    current.Parent.SafelyAddChild(this);
                }

                if (hasLink && ElementName == "a")
                {
                    bool nextLink = false;
                    foreach (var attribute in Attributes)
                    {
                        if (nextLink && attribute as ElementAttributeValue != null)
                        {
                            string tempLink = fixLink(attribute.html.ToString());
                            if (tempLink != null)
                                Page.PageLinks.Add(tempLink);
                            nextLink = false;
                        }
                        if (attribute.html.ToString().ToLower() == "href")
                            nextLink = true;
                    }
                }
            }

            for (int j = possibles.Count - 1; j >= 0; j--)
            {
                possibles[j].AddCharacter(lastChar, currentChar, nextChar);

                if (possibles[j].IsCompleted)
                {
                    if (possibles[j].html.ToString().ToLower() == "href")
                        hasLink = true;

                    if (possibles[j] as ElementAttributeValue != null)
                    {
                        inValue = false;
                        nextIsValue = false;
                    }

                    Attributes.Add(possibles[j]);
                    possibles.Remove(possibles[j]);
                }
            }

            if (!inValue)
                possibles.AddRange(ObjectFactory(lastChar, currentChar, nextChar));
        }
        string fixLink(string link)
        {
            link = link.ToLower();
            if (link.StartsWith("java") || link.StartsWith("#"))
                return null;
            if (link.StartsWith("http"))
            {
                return link;
            } else if (link.StartsWith("//"))
            {
                return "http:" + link;
            } else
            {
                Uri baseUri = new Uri("http://" + Page.Url.Host);
                return new Uri(baseUri, link).ToString();
            }
        }

        public override PageObject Create(char? lastChar, char currentChar, char? nextChar)
        {
            if (this.IsPossibleStart(lastChar, currentChar, nextChar))
            {
                PageElement result = new PageElement(this.Page);
                result.html.Append(currentChar);

                //if (nextChar.HasValue && nextChar.Value != '/')
                //    result.OpeningTag = true;
                return result;
            }
            return null;
        }

        public void SafelyChangeParent(PageElement parent)
        {
            if (this.Parent != null)
                this.Parent.Children.Remove(this);
            this.Parent = parent;
        }

        public void SafelyAddChild(PageObject child)
        {
            PageElement tempPE = child as PageElement;
            if (tempPE != null)
                tempPE.SafelyChangeParent(this);
            Children.Add(child);
        }
        public void AddChildren(List<PageObject> children)
        {
            // we do a backwards for loop as list handed may be one that gets deleted from
            for (int i = children.Count - 1; i >= 0; i--)
            {
                SafelyAddChild(children[i]);
            }
        }

        List<PageObject> ObjectFactory(char? lastChar, char currentChar, char? nextChar)
        {
            List<PageObject> objects = new List<FoogleEngine.PageObject>();
            foreach (var factory in factories)
            {
                PageObject temp = factory.Create(lastChar, currentChar, nextChar);

                // Elements may think they can start inside a element value
                // Element values may think the end of one element value is the beginning of another
                ElementAttributeValue tempEAV = temp as ElementAttributeValue;
                if (tempEAV != null && nextIsValue)
                    inValue = true;

                if (temp != null && (tempEAV == null || inValue))
                    objects.Add(temp);
            }
            return objects;
        }
    }
}
