﻿using System.Collections.Generic;
using System.Linq;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Shared
{
    public class SelectionTemplate
    {
        private static readonly List<char> _showedChars = new List<char> {'\'', '-'};

        public SelectionTemplate(string selection)
        {
            var singleWords = selection.Split(" ").ToList();
            singleWords.ForEach(word =>
            {
                var characters = word.ToCharArray().ToList();
                characters.ForEach(ch =>
                {
                    if (_showedChars.Contains(ch))
                    {
                        Characters.Add(ch);
                    }
                    else
                    {
                        Characters.Add('_');
                    }
                });
                Characters.Add(' ');
            });
        }

        public SelectionTemplate()
        {
        }

        public List<char> Characters { get; set; } = new List<char>();

        public void AddHintLetter(HintDto hint)
        {
            Characters[hint.Index] = hint.Letter;
        }
    }
}