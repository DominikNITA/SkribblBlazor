using System.Timers;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Models
{
    public class HintTimer : Timer
    {
        public HintTimer(HintDto hint, double interval) : base(interval)
        {
            Hint = hint;
        }

        public HintDto Hint { get; }
    }
}