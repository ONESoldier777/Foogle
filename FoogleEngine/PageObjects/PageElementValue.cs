using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class PageElementValue : PageObject
    {
        public PageElementValue(Page page) : base(page)
        {
            this.StartingLastCharacterFilters.Add('>');
            this.EndingNextCharacterFilters.Add('<');
        }
        public override void AddCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            html.Append(currentChar);
            if (IsEndingCharacter(lastChar, currentChar, nextChar))
                IsCompleted = true;
        }
        public override PageObject Create(char? lastChar, char currentChar, char? nextChar)
        {
            if (lastChar.HasValue && this.IsPossibleStart(lastChar, currentChar, nextChar))
            {
                PageElementValue result = new FoogleEngine.PageElementValue(this.Page);
                result.html.Append(currentChar);
                return result;
            }
            return null;
        }

        public override bool IsPossibleStart(char? lastChar, char currentChar, char? nextChar)
        {
            if (currentChar != '<')
            {
                return base.IsPossibleStart(lastChar, currentChar, nextChar);
            }
            return false;
        }
    }
}
