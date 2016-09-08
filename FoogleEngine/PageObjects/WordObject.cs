using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class WordObject : PageObject
    {
        public WordObject(Page page) : base(page)
        {
            EndingCharacterFilters.AddRange(new char[] { ' ', ' ' });
        }

        public override void AddCharacter(char? lastChar, char currentChar, char? nextChar)
        {
            throw new NotImplementedException();
        }

        public override PageObject Create(char? lastChar, char currentChar, char? nextChar)
        {
            throw new NotImplementedException();
        }
    }
}
