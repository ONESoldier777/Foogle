using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class WhitespaceObject : PageObject
    {
        public WhitespaceObject(Page page) : base(page)
        {
            StartingCharacterFilters.Add(' ');
        }

        public override void AddCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            if (IsCompleted)
                return;

            html.Append(currentChar);
            if (IsEndingCharacter(lastChar, currentChar, nextChar))
                IsCompleted = true;
        }

        public override PageObject Create(char? lastChar, char currentChar, char? nextChar)
        {
            if (this.IsPossibleStart(lastChar, currentChar, nextChar))
            {
                WhitespaceObject result = new WhitespaceObject(Page);
                result.html.Append(currentChar);
                if (IsEndingCharacter(lastChar, currentChar, nextChar))
                    result.IsCompleted = true;
                return result;
            }
            return null;
        }
        public override bool IsEndingCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            if (!nextChar.HasValue || nextChar.Value != ' ')
            {
                return true;
            }
            return false;
        }
    }
}
