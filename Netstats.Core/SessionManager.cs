using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Netstats.Core.Interfaces;
using Netstats.Network.Api.Interfaces;
using Netstats.Network.Api;
using Newtonsoft.Json;

namespace Netstats.Core
{
    public class SessionManager : ISessionManager
    {
        private ISession currentSession;

        private SemaphoreSlim LoginGate { get; }

        public SessionManager(INetworkApi networkApi)
        {
            NetworkApi = networkApi;
            LoginGate = new SemaphoreSlim(1, 1);
        }

        public SessionManager(DummyNetworkApi dummyNetworkApi, Type type)
        {
            this.dummyNetworkApi = dummyNetworkApi;
            this.type = type;
        }

        public ISession CurrentSession { get { return currentSession; } }

        public INetworkApi NetworkApi { get; set; }

        public bool HasActiveSession { get { return CurrentSession != null; } }

        /*very basic validation*/
        public Func<string, string, bool> ValidateLoginCredentials = (username, password) =>
           !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);

        public async Task<ISession> CreateSession(string username, string password, CancellationToken token)
        {
            LoginGate.Wait();

            if (!ValidateLoginCredentials(username, password))
                throw new ArgumentException("user credentials");

            /*We don't want any lingering sessions. This is not entirely necessary but it's nice
            to keep things tidy*/
            if (HasActiveSession)
                await DestroyCurrentSessionAsync();

            try
            {
                var json = await NetworkApi.Login(username, password, token);
                // Parse id token from json response
                var response = JsonConvert.DeserializeAnonymousType(json, new { Id = string.Empty });

                currentSession = new UserSenseSession(response.Id, new CoreSettings(), NetworkApi);

                return currentSession;
            }
            catch (Exception ex)
            {
                // To-do repelace with better loggin mechanism
                Debug.WriteLine("unable to login" + ex.Message);
                throw;
            }
            finally
            {
                LoginGate.Release();
            }
        }

        public Task<ISession> CreateSession(string username, string password) => CreateSession(username, password, CancellationToken.None);

        public async Task DestroyCurrentSessionAsync()
        {
            await NetworkApi.Logout(currentSession.Id);
            currentSession = null;
        }

        #region Static Members

        static Lazy<SessionManager> instance = new Lazy<SessionManager>(() =>
        {
            return new SessionManager(new DummyNetworkApi());
        });
        private DummyNetworkApi dummyNetworkApi;
        private Type type;

        public static SessionManager Instance { get { return instance.Value; } }

        #endregion
    }
}
