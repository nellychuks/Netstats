namespace Netstats.Network
{
    public class LoginResult : NetworkResult
    {
        public int SessionId { get; set; }

        public LoginResult(bool success = true, NetworkErrorKind error = NetworkErrorKind.None) : base(success, error)
        {

        }
    }
}
