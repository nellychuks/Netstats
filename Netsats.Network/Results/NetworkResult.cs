namespace Netstats.Network
{
    public abstract class NetworkResult
    {
        public bool TaskSucceded { get; }

        public NetworkErrorKind ErrorKind { get; }

        public NetworkResult(bool success = true, NetworkErrorKind error = NetworkErrorKind.None)
        {
            TaskSucceded = success;
            ErrorKind = error;
        }
    }
}
