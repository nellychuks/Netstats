using Netstats.Network.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Netstats.Network.Services
{
    public class LoginService : ILoginService
    {
        INetworkApi NetworkApi;

        public LoginService(): this(new UserSenseApi()) { }

        public LoginService(INetworkApi networkApi)
        {
            NetworkApi = networkApi ?? new UserSenseApi();
        }

        public async Task<LoginResult> Login(string username, string password, CancellationToken cancelToken)
        {
            var response = await NetworkApi.Login(username, password, cancelToken);
            if (response == null)
                throw new ServerConnectionFailedException();

            var pageKind = Utilities.IdentifyPage(response);
            if (pageKind == PageKind.SessionPage)
            {
                return new LoginResult(Utilities.ParseSessionId(response), Utilities.ParseSessionFeed(response));
            }
            else if (pageKind == PageKind.ConfirmationPage)
            {
                response = await NetworkApi.ConfirmLogin(username, password, cancelToken);
                if (Utilities.IdentifyPage(response) == PageKind.SessionPage)
                    return new LoginResult(Utilities.ParseSessionId(response), Utilities.ParseSessionFeed(response));
                throw new LoginFailedException();
            }
            else if (pageKind == PageKind.AuthenticationFailedPage)
            {
                throw new LoginFailedException(LoginFailReason.AuthenticationFailed);
            }
            else
            { 
                throw new LoginFailedException(LoginFailReason.Unkown);
            }
        }

        public async Task Logout(string sessionId)
        {
            await NetworkApi.Logout(sessionId);
        }
    }
}
