using Lager;
using System;
using Akavache;

namespace Netstats.Core
{
    public class CoreSettings : SettingsStorage
    {
        static CoreSettings()
        {
            BlobCache.ApplicationName = "Netstats";
        }

        public CoreSettings(IBlobCache cache = null) : base("6967faf0-2c99-4bc6-9da0-4c289bd7e48c", cache ?? BlobCache.LocalMachine)
        {

        }

        public TimeSpan RefreshInterval
        {
            get { return GetOrCreate(TimeSpan.FromSeconds(2)); }
            set { SetOrCreate(value); }
        }
    }
}
