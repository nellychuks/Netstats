using System;
using System.Threading;
using System.Threading.Tasks;
using Netstats.Core.Api.Exceptions;
using Netstats.Core.Management.Interfaces;
using Netstats.Core.Network;
using Netstats.Core.Network.Api;

namespace Netstats.Core.Management
{
    public class SessionManager: ISessionManager
    {
        ISession activeSession;
        SemaphoreSlim gate;

        public SessionManager(INetworkApi networkApi)
        {
            NetworkApi = networkApi;
            gate = new SemaphoreSlim(1, 1);
        }
        
        public ISession ActiveSession { get { return activeSession; } set { activeSession = value; } }
        public INetworkApi NetworkApi { get; set; }
        public bool HasActiveSession { get { return activeSession != null; } }
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(3);

        /*very basic validation*/
        public Func<string, string, bool> ValidateLoginCredentials = (username, password) =>
           !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);

        public async Task<ISession> CreateSession(string username, string password, CancellationToken token)
        {
            /*If you don't acquire the lock in aboout 50ms, it's probably in use
            This is solely to reduce concurrent requests*/
            if (!await gate.WaitAsync(50))
                throw new Exception("Already processing a request");

            if (!ValidateLoginCredentials(username, password))
                throw new ArgumentException("user credentials");
            /*We don't want any lingering sessions. This is not entirely necessary but it's nice
            to keep things tidy*/
            if (HasActiveSession)
                await DestroyActiveSession();
            ActiveSession = await Login(username, password, token);
            gate.Release();
            return ActiveSession;
        }

        public async Task<ISession> CreateSession(string username, string password)
        {
            /*If you don't acquire the lock in aboout 50ms, it's probably in use
            This is solely to reduce concurrent requests*/
            await gate.WaitAsync(50, CancellationToken.None);
            if (!ValidateLoginCredentials(username, password))
                throw new ArgumentException("user credentials");
            /*We don't want any lingering sessions. This is not entirely necessary but it's nice
            to keep things tidy*/
            if (HasActiveSession)
                await DestroyActiveSession();
            ActiveSession = await Login(username, password, CancellationToken.None);
            gate.Release();
            return ActiveSession;
        }

        public async Task DestroyActiveSession()
        {
            if (!HasActiveSession)
                return;
            try
            {
                if (!await gate.WaitAsync(50))
                    return;
                await NetworkApi.Logout(ActiveSession.Id);
                ActiveSession = null;
            }
            catch
            { /*Swallow all errors*/ }
            finally { gate.Release(); }
        }

        private async Task<ISession> Login(string username, string password, CancellationToken token)
        {
            IPage response = null;

            try
            {
                var responsestring = await NetworkApi.Login(username, password, token);
                response = PageMixins.ParsePage(responsestring);

                /*Process confirm action*/
                if (response.Kind == PageKind.ConfirmationPage)
                {
                    responsestring = await NetworkApi.Login(username, password, token);
                    response = PageMixins.ParsePage(responsestring);
                }   
            }
            catch (ServerConnectionFailedException ex)
            {
                throw new LoginFailedException(SessionCreateFailReaon.UnableToReachServer, ex);
            }
            catch(Exception ex)
            {
                throw new LoginFailedException(SessionCreateFailReaon.Unknown, ex);
            }
            ThrowIfNotSessionPage(response);
            return new UserSenseSession(PageMixins.ParseId(response), NetworkApi);
        }

        public void ThrowIfNotSessionPage(IPage page)
        {
            if (page.Kind == PageKind.AuthenticationFailedPage)
                throw new LoginFailedException(SessionCreateFailReaon.AuthenticationFailed);

            else if (page.Kind == PageKind.BandwidthExceededPage)
                throw new LoginFailedException(SessionCreateFailReaon.BandwidthExceeded);

            else if (page.Kind == PageKind.MaxUserSessionsReached)
                throw new LoginFailedException(SessionCreateFailReaon.MaxSessionsReached);
        }

        static Lazy<SessionManager> instance = new Lazy<SessionManager>(() => new SessionManager(new DummyApi()));
        public static SessionManager Instance { get { return instance.Value; } }
    }


    public enum BandwidthQuotaType
    {
        Weekly,

        Monthly,

        Yearly,

        Unknown
    }

    public enum SessionCreateFailReaon
    {
        Unknown,

        AuthenticationFailed,

        BandwidthExceeded,

        MaxSessionsReached,

        UnableToReachServer
    }
}
