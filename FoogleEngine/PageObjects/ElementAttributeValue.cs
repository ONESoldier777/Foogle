using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class ElementAttributeValue : PageObject
    {
        public ElementAttributeValue(Page page) : base(page)
        {
            StartingLastCharacterFilters.Add('"');
            EndingNextCharacterFilters.Add('"');
            //EndingNextCharacterFilters.AddRange(new char[] { ' ', '/', '>' });
        }

        public override void AddCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            html.Append(currentChar);
            if (IsEndingCharacter(lastChar, currentChar, nextChar))
                IsCompleted = true;
        }

        public override PageObject Create(char? lastChar, char currentChar, char? nextChar)
        {
            if (this.IsPossibleStart(lastChar, currentChar, nextChar))
            {
                ElementAttributeValue result = new ElementAttributeValue(Page);
                result.html.Append(currentChar);
                return result;
            }
            return null;
        }
    }
}
