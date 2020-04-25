using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Interfaces
{
    public interface IWordDistanceCalculator
    {
        int Calculate(string word, string target);
    }
}
