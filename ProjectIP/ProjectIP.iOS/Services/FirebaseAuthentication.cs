using Firebase.Auth;
using Foundation;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ProjectIP.iOS.Services
{
    class FirebaseAuthentication : IAuthenticationService
    {
        public async Task<string> GetToken()
        {
            if (IsSignedIn())
            {
                return await Auth.DefaultInstance.CurrentUser.GetIdTokenAsync();
            }
            return string.Empty;
        }

        public string GetUid()
        {
            if (IsSignedIn())
            {
                return Auth.DefaultInstance.CurrentUser.Uid;
            }
            return string.Empty;
        }

        public bool IsSignedIn()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            return user != null;
        }

        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception)
            {
                return string.Empty;
            }
           

        }

        public async Task<string> RegisterWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.CreateUserAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool SignOut()
        {
            try
            {
                _ = Auth.DefaultInstance.SignOut(out NSError error);
                return error == null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}