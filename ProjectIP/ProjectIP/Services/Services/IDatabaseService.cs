using ProjectIP.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectIP.Interfaces
{
    public interface IDatabaseService
    {
        Task<List<Word>> GetAllWords();
        Task<bool> DeleteWord(Word word);
        Task AddWord(Word newWord);
        Task EditWord(Word editedWord);
    }
}
