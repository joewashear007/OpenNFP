using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public enum ClearBlueResult
    {
        Unknown = 0,
        Low = 1,
        High = 2,
        Peak = 3
    }

    public enum TestResult
    {
        Unknown = 0,
        Positive = 1,
        Negative = 2,
    }

    public enum MenstruationFlow
    {
        Unknown = 0,
        Spotting = 1,
        Light = 2,
        Heavy = 3
    }

    public enum CervixOpening
    {
        Unknown = 0,
        Closed = 1,
        Partial = 2,
        Open = 3
    }
    public enum CervixTexture
    {
        Unknown = 0,
        Firm = 1,
        Soft = 2
    }

    public enum MucusSensation
    {
        Unknown = 0,
        None = 1,
        Most = 2,
        Slippery = 3,
        Watery = 4,
    }

    public enum MucusCharacteristic
    {
        Unknown = 0,
        None = 1,
        Tacky = 2,
        Strechy = 3,
    }
}
