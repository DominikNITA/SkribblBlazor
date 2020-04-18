using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    //http://www.desiquintans.com/nounlist
    public class WordsProviderService : IWordsProviderService
    {
        private List<string> _words = new List<string>();

        public WordsProviderService()
        {
            PopulateWordsList();
        }

        private void PopulateWordsList()
        {
            //http://www.desiquintans.com/downloads/nounlist/nounlist.txt
            string path = "../Skribbl_Website/Server/Data/english_words.txt";
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                path = "Data/english_words.txt";
            }
            string line;
            var file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                _words.Add(line);
            }
        }

        public async Task<List<string>> GetWords()
        {
            var random = new Random();
            int wordsCount = 3;
            var result = new List<string>();
            for (int i = 0; i < wordsCount; i++)
            {
                result.Add(_words[random.Next(_words.Count)]);
            }
            return result;
        }
    }
}
