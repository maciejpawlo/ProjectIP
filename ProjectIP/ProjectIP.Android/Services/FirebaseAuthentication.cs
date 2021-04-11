using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectIP.Droid.Services
{
    class FirebaseAuthentication : IAuthenticationService
    {
        public async Task<string> GetToken()
        {
            if (IsSignedIn())
            {
                GetTokenResult tokenRequest = (GetTokenResult) await FirebaseAuth.Instance.CurrentUser.GetIdToken(true);
                var test = tokenRequest.GetType();
                return tokenRequest.Token;
            }
            return string.Empty;
        }

        public string GetUid()
        {
            if (IsSignedIn())
            {
                return FirebaseAuth.Instance.CurrentUser.Uid;
            }
            return string.Empty;
        }

        public bool IsSignedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            
            return user != null;
        }

        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = user.User.Uid;
                return token;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public async Task<string> SignInWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var token = user.User.Uid;
                return token;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public bool SignOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}