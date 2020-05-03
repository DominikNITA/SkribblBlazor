namespace Skribbl_Website.Shared.Dtos
{
    public class HintDto
    {
        public HintDto()
        {
        }

        public HintDto(int index, char letter)
        {
            Index = index;
            Letter = letter;
        }

        public int Index { get; set; }
        public char Letter { get; set; }
    }
}