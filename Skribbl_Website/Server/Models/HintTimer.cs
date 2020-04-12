using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Skribbl_Website.Server.Models
{
    public class HintTimer : Timer
    {
        public HintDto Hint { get; private set; }

        public HintTimer(HintDto hint, double interval) : base(interval)
        {
            Hint = hint;
        }
    }
}
