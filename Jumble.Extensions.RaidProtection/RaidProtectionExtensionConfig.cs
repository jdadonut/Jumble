using System.Collections.Generic;

namespace Jumble.Extensions.RaidProtection
{
    public struct RaidProtectionExtensionConfig
    {
        #nullable enable
        public bool UseServerSettings;
        public string? SqliteDatabasePath;
    }
}