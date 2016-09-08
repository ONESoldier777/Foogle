using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public abstract class PageObject
    {
        protected Page Page;

        public int Index;
        public int Length;
        
        public StringBuilder html = new StringBuilder();

        public bool IsOK = true;
        public bool IsCompleted = false;

        public List<char> StartingLastCharacterFilters = new List<char>();
        public List<char> StartingCharacterFilters = new List<char>();

        public List<char> EndingCharacterFilters = new List<char>();
        public List<char> EndingNextCharacterFilters = new List<char>();

        public PageObject(Page page)
        {
            this.Page = page;
        }
        abstract public void AddCharacter(char? lastChar, char currentChar, char? nextChar);
        abstract public PageObject Create(char? lastChar, char currentChar, char? nextChar);

        virtual public bool IsPossibleStart(char? lastChar, char currentChar, char? nextChar)
        {
            if ((!lastChar.HasValue || StartingLastCharacterFilters.Count == 0 || StartingLastCharacterFilters.BinarySearch(lastChar.Value) >= 0) &&
                (StartingCharacterFilters.Count == 0 || StartingCharacterFilters.BinarySearch(currentChar) >= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        virtual public bool IsEndingCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            if ((EndingCharacterFilters.Count == 0 || EndingCharacterFilters.BinarySearch(currentChar) >= 0) &&
                (!nextChar.HasValue || EndingNextCharacterFilters.Count == 0 || EndingNextCharacterFilters.BinarySearch(nextChar.Value) >= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
