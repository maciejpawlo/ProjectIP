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
            var newWordRef = await DbClient.Child("word").Child(uid).PostAsync(newWord);
            await DbClient.Child("word").Child(uid).Child(newWordRef.Key).PatchAsync(new { ID = newWordRef.Key });
        }

        public async Task<bool> DeleteWord(Word word)
        {
            //sprawdzać obiekt usuwany tylko z poddrzewa prywatnego 
            if (word.ID == null)
            {
                return false; //jesli ID==null -> obiekt z drzewa shared
            }
            var uid = _authenticationService.GetUid();
            await DbClient.Child("word").Child(uid).Child(word.ID).DeleteAsync();
            return true;
        }

        public async Task EditWord(Word word)
        {
            //przy edycji słowa z shared -> przepisanie na prywatne poddrzewo
            var uid = _authenticationService.GetUid();
            if (word.ID == null) //id null jesli obiekt pochodzi z drzewa shared 
            {
                var newWordRef = await DbClient.Child("word").Child(uid).PostAsync(word);
                await DbClient.Child("word").Child(uid).Child(newWordRef.Key).PatchAsync(new { ID = newWordRef.Key });
            }
            else //edycja zwyklego PRYWATNEGO obiektu
            {
                await DbClient.Child("word").Child(uid).Child(word.ID).PutAsync(word);
            }           
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
