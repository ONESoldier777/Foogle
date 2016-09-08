using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class PageParser
    {
        internal Page Page;
        internal List<PageObject> factories;

        public PageParser(Page page)
        {
            this.Page = page;
            factories = new List<PageObject>
            {
                new PageElement(Page),
                new PageElementValue(Page)
            };
        }
        internal void Parse(string pageString)
        {
            // This goes one character at a time through the entire html string
            // Its initial use is to determine text areas as either elements or element values
            // It will also handle parenting these elements

            Page.Children.Clear();

            PageElement tempContainer = new PageElement(Page);
            Page.ElementsStack.Push(tempContainer);
            List<PageObject> currentObjects = new List<FoogleEngine.PageObject>();
            //List<PageObject> completedObjects = new List<PageObject>();
            char? lastChar = null;
            char currentChar;
            char? nextChar = null;
            for (int i = 0; i < pageString.Length; i++)
            {
                currentChar = pageString[i];
                if ((i + 1) == pageString.Length)
                    nextChar = null;
                else
                    nextChar = pageString[i + 1];

                for (int j = currentObjects.Count - 1; j >= 0; j--)
                {
                    currentObjects[j].AddCharacter(lastChar, currentChar, nextChar);

                    if (currentObjects[j].IsCompleted)
                    {
                        if (currentObjects[j] as PageElement == null || ((PageElement)currentObjects[j]).TagType != PageElement.PageElementTagType.Closing)
                        {
                            tempContainer.SafelyAddChild(currentObjects[j]);

                            if (currentObjects[j] as PageElementValue != null)
                            {
                                if (tempContainer.ElementName != "script")
                                {
                                    Page.SearchableText += currentObjects[j].html.ToString().ToLower();
                                }
                                if (tempContainer.ElementName == "title")
                                {
                                    Page.Title += currentObjects[j].html.ToString();
                                }
                            }
                        }
                        tempContainer = Page.ElementsStack.Peek();
                        //Page.ElementsStack.Peek().Children.Add(currentObjects[j]);
                        // if currentParent is null set it to first page element

                    }

                    if (currentObjects[j].IsCompleted || !currentObjects[j].IsOK)
                        currentObjects.Remove(currentObjects[j]);
                }


                currentObjects.AddRange(ObjectFactory(lastChar, currentChar, nextChar));

                lastChar = currentChar;
            }

            tempContainer.Parent = null;
            tempContainer.TagType = PageElement.PageElementTagType.Broken;
            for (int i = tempContainer.Children.Count-1; i >= 0; i--)
            {
                Page.Children.Add(tempContainer.Children[i]);
                PageElement tempPE = tempContainer.Children[i] as PageElement;
                if (tempPE != null) {
                    tempPE.Parent = null;
                }
                tempContainer.Children.Remove(tempContainer.Children[i]);
            }
            Page.Children.Add(tempContainer);
            Page.Children.Reverse();
        }
        internal List<PageObject> ObjectFactory(char? lastChar, char currentChar, char? nextChar)
        {
            List<PageObject> objects = new List<FoogleEngine.PageObject>();
            foreach (var factory in factories)
            {
                PageObject temp = factory.Create(lastChar, currentChar, nextChar);
                if (temp != null)
                    objects.Add(temp);
            }
            return objects;
        }
    }
}
