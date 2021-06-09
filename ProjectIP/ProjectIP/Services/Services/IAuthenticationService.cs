using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectIP.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> LoginWithEmailAndPassword(string email, string password);
        Task<string> RegisterWithEmailAndPassword(string email, string password);
        bool SignOut();
        bool IsSignedIn();
        Task<string> GetToken();
        string GetUid();
    }
}
