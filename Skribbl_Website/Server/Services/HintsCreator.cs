using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    public static class HintsCreator
    {
        public static List<char> OmittedCharacters = new List<char> { ' ', '_', '\'' };
        private static readonly int _minimumTimeLeftAfterLastHint = 10000;
        public static List<HintTimer> CreateHintTimersForSelection(string selection,int timeLimit)
        {
            int selectionLength = selection.Length;
            if(selectionLength < 4)
            {
                return GetSpecificAmountOfHints(selection, 1,timeLimit);
            }
            else if(selectionLength < 8)
            {
                return GetSpecificAmountOfHints(selection, 2, timeLimit);
            }
            else
            {
                return GetSpecificAmountOfHints(selection, 3, timeLimit);
            }
        }

        private static List<HintTimer> GetSpecificAmountOfHints(string selection, int amount, int timeLimit)
        {
            var timers = new List<HintTimer>();
            while(timers.Count < amount)
            {
                Random random = new Random();
                int index = random.Next(selection.Length);
                char hintLetter = selection[index];
                if (!OmittedCharacters.Contains(hintLetter) || timers.Any(timer => timer.Hint.Index == index)){
                    timers.Add(new HintTimer(new HintDto(index, hintLetter), GetIntervalForTimer(timers.Count+1, amount, timeLimit*1000)));
                }
            }
            return timers;
        }

        private static int GetIntervalForTimer(int timerNumber, int targetAmount, int timeLimit)
        {
            return (timeLimit*2/3 - _minimumTimeLeftAfterLastHint) * timerNumber / targetAmount;
        }
    }
}
