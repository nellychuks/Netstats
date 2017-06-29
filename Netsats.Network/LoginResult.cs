namespace Netstats.Network
{
    public class LoginResult 
    {
        public int SessionId { get; set; }
        public SessionFeed Feed { get; }

        public LoginResult(int sessionId, SessionFeed feed) 
        {
            SessionId = sessionId;
            Feed = feed; 
        }
    }
}
