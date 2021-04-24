using Firebase.Database;
using Firebase.Database.Query;
using ProjectIP.Interfaces;
using ProjectIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Ioc;
using System.Text;
using System.Threading.Tasks;

namespace ProjectIP.Services
{
    public class DatabaseService : IDatabaseService
    {
        public IAuthenticationService _authenticationService { get; private set; }
        private FirebaseClient DbClient { get;  set; }
        public DatabaseService(IAuthenticationService authenticationService)
        {
             _authenticationService = authenticationService;
            DbClient = App.Current.Container.Resolve<FirebaseClient>();
        }
        public async Task AddWord(Word newWord)
        {
            var uid = _authenticationService.GetUid();
            await DbClient.Child("word").Child(uid).Child(newWord.Description).PutAsync(newWord);
        }

        public async Task DeleteWord(string path)
        {
            //TODO sprawdzać obiekt usuwany tylko z poddrzewa prywatnego 
            //if path not contains shared --> nie usuwaj 
            throw new NotImplementedException();
        }

        public async Task EditWord(string path)
        {
            //TODO przy edycji słowa z shared -> przepisanie na prywatne poddrzewo
            throw new NotImplementedException();
        }

        public async Task<List<Word>> GetAllWords()
        {
            var uid = _authenticationService.GetUid();
            var shared = (await DbClient.Child("word").Child("shared").OnceAsync<Word>()).Select(x => x.Object).ToList();
            var words = (await DbClient.Child("word").Child(uid).OnceAsync<Word>()).Select(x => x.Object).ToList();
            var query = (from sh in shared
                         join pv in words on new { sh.Description, sh.Category } equals new { pv.Description, pv.Category }
                         select sh).ToList();
            var concat = words.Concat(shared).ToList();
            foreach (var item in query)
            {
                concat.Remove(item);
            }
            return concat;
        }
    }
}
