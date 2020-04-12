using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    //http://www.desiquintans.com/nounlist
    public class WordsProviderService : IWordsProviderService
    {
        public async Task<List<string>> GetWords()
        {
            return new List<string> { "dog", "cat", "fridge" };
        }
    }
}
