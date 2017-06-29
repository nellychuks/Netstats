using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Netstats.Core.Management
{
    public class UserCredentialsStore
    {
        public int UsersCount { get { return GetUserNames().ToEnumerable().Count(); } }

        public IObservable<IEnumerable<User>> GetUsers()
        {
            return BlobCache.Secure.GetAllObjects<User>();
        }

        public IObservable<IEnumerable<string>> GetUserNames()
        {
            return BlobCache.Secure.GetAllKeys();
        }

        public IObservable<User> GeUser(String alias)
        {
            return BlobCache.Secure.GetObject<User>(alias);
        }

        public IObservable<Unit> AddUser(User user) => BlobCache.Secure.InsertObject(user.Alias, user);

        public IObservable<Unit> DeleteUser(User user) => BlobCache.Secure.Invalidate(user.Alias);

        public IObservable<Unit> DeleteAllusers() => BlobCache.Secure.InvalidateAllObjects<User>();

        public IObservable<bool> HasUserAlias(string alias) => GetUserNames().Any(x => x.Contains(alias));

        public IObservable<bool> HasUsername(string username)
        {
            foreach (var user in GetUsers().First())
            {
                if (user.Username == username)
                    return Observable.Return(true);
            }
            return Observable.Return(false);
        }

        public void ShutDown() => BlobCache.Shutdown().Wait();

        #region Security

        public IObservable<bool> IsEntryPinSet()
        {
            return BlobCache.Secure.GetAllKeys().Any(x => x.Contains("entry_pin"));
        }

        public IObservable<Unit> SetEntryPin(string pin)
        {
            return BlobCache.Secure.InsertObject("entry_pin", pin);
        }

        public IObservable<string> GetEntryPin()
        {
            return BlobCache.Secure.GetObject<string>("entry_pin");
        }

        private static Lazy<UserCredentialsStore> instance = new Lazy<UserCredentialsStore>();
        public static UserCredentialsStore Instance { get { return instance.Value; } }

        #endregion
    }


    public class User
    {
        public string Alias { get; }
        public string Username { get; }
        public string Password { get; }

        public User(string alias, string username, string password)
        {
            Username = username;
            Password = password;
            Alias = alias;
        }
    }
}
