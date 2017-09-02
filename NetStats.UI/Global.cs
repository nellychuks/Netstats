using Netstats.Core;
using System;
using System.Collections.Generic;

namespace Netstats.UI
{
    static class Global
    {
        public static string CurrentUser { get; set; }
        public static bool IsLoggedIn { get { return SessionManager.Instance.HasActiveSession; } }
        public static bool IsAppLocked { get; set; }
        public static Stack<Action> OnNextUnlockActions { get; } = new Stack<Action>();
    }
}
