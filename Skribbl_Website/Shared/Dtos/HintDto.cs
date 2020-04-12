using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class HintDto
    {
        public int Index { get; set; }
        public char Letter { get; set; }
        public HintDto()
        {

        }

        public HintDto(int index, char letter)
        {
            Index = index;
            Letter = letter;
        }
    }
}
