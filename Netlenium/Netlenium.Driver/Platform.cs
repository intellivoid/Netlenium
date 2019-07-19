using System;

namespace Netlenium.Driver
{
    /// <summary>
    ///     The platform type
    /// </summary>
    [Flags]
    public enum Platform
    {
        None = 0,
        AutoDetect = 1,
        Windows32 = 2,
        Windows64 = 3,
        Linux32 = 4,
        Linux64 = 5
    }
}