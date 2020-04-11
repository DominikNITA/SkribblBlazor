using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    public interface IWordsProviderService
    {
        Task<List<string>> GetWords();
    }
}