using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public static class EnumMapper
    {
        public static (double value, string label) CombineCervixValue(CervixOpening opening, CervixTexture texture)
        {
            string label = opening.ToChartLabel();

            if (opening != CervixOpening.Unknown && texture != CervixTexture.Unknown)
            {
                label += "/" + texture.ToChartLabel();
            }
            else
            {
                label += texture.ToChartLabel();
            }

            return (Math.Max(opening.ToChartValue(), texture.ToChartValue()), label);
        }

        public static (double value, string label) CombineMucusValue(MucusSensation value1, MucusCharacteristic value2)
        {
            string label = value1.ToChartLabel();

            if (value1 != MucusSensation.Unknown && value2 != MucusCharacteristic.Unknown)
            {
                label += "/" + value2.ToChartLabel();
            }
            else
            {
                label += value2.ToChartLabel();
            }

            return (Math.Max(value1.ToChartValue(), value2.ToChartValue()), label);
        }

        public static double ToChartValue(this ClearBlueResult value)
        {
            switch (value)
            {
                case ClearBlueResult.Unknown: return 0.0;
                case ClearBlueResult.Low: return 0.15;
                case ClearBlueResult.High: return 0.5;
                case ClearBlueResult.Peak: return 1.0;
                default: return 0.0;
            }
        }

        public static string ToChartLabel(this ClearBlueResult value)
        {
            string label = "";
            switch (value)
            {
                case ClearBlueResult.Unknown: break;
                case ClearBlueResult.Low: label += "L"; break;
                case ClearBlueResult.High: label += "H"; break;
                case ClearBlueResult.Peak: label += "P"; break;
            }
            return label;

        }

        public static double ToChartValue(this CervixOpening value)
        {
            switch (value)
            {
                case CervixOpening.Unknown: return 0.0;
                case CervixOpening.Closed: return 0.15;
                case CervixOpening.Partial: return 0.5;
                case CervixOpening.Open: return 1.0;
                default: return 0.0;
            }
        }

        public static string ToChartLabel(this CervixOpening value)
        {
            string label = "";
            switch (value)
            {
                case CervixOpening.Unknown: break;
                case CervixOpening.Closed: label += "●"; break;
                case CervixOpening.Partial: label += "○"; break;
                case CervixOpening.Open: label += "◯"; break;
            }
            return label;

        }

        public static double ToChartValue(this CervixTexture value)
        {
            switch (value)
            {
                case CervixTexture.Unknown: return 0.0;
                case CervixTexture.Firm: return 0.5;
                case CervixTexture.Soft: return 0.1;
                default: return 0.0;
            }
        }

        public static string ToChartLabel(this CervixTexture value)
        {
            string label = "";
            switch (value)
            {
                case CervixTexture.Unknown: break;
                case CervixTexture.Firm: label += "F"; break;
                case CervixTexture.Soft: label += "S"; break;
            }
            return label;

        }

        public static double ToChartValue(this MucusSensation value)
        {
            switch (value)
            {
                case MucusSensation.Unknown: return 0.0;
                case MucusSensation.None: return 0.0;
                case MucusSensation.Most: return 0.1;
                case MucusSensation.Watery: return 0.5;
                case MucusSensation.Slippery: return 1.0;
                default: return 0.0;
            }
        }
        public static string ToChartLabel(this MucusSensation value)
        {
            string label = "";
            switch (value)
            {
                case MucusSensation.Unknown: break;
                case MucusSensation.None: label += "d"; break;
                case MucusSensation.Most: label += "m"; break;
                case MucusSensation.Watery: label += "w"; break;
                case MucusSensation.Slippery: label += "sl "; break;
            }
            return label;
        }

        public static double ToChartValue(this MucusCharacteristic value)
        {
            switch (value)
            {
                case MucusCharacteristic.Unknown: return 0.0;
                case MucusCharacteristic.None: return 0.0;
                case MucusCharacteristic.Tacky: return 0.5;
                case MucusCharacteristic.Strechy: return 1.0;
                default: return 0.0;
            }
        }

        public static string ToChartLabel(this MucusCharacteristic value)
        {
            string label = "";
            switch (value)
            {
                case MucusCharacteristic.Unknown: break;
                case MucusCharacteristic.None: label += "n"; break;
                case MucusCharacteristic.Tacky: label += "t"; break;
                case MucusCharacteristic.Strechy: label += "s"; break;
            }
            return label.Trim();
        }

        public static double ToChartValue(this MenstruationFlow value)
        {
            switch (value)
            {
                case MenstruationFlow.Unknown: return 0.0;
                case MenstruationFlow.Spotting: return 0.15;
                case MenstruationFlow.Light: return 0.5;
                case MenstruationFlow.Heavy: return 1.0;
                default: return 0.0;
            }
        }

        public static string ToChartLabel(this MenstruationFlow value)
        {
            string label = "";
            switch (value)
            {
                case MenstruationFlow.Unknown: break;
                case MenstruationFlow.Spotting: label += "●"; break;
                case MenstruationFlow.Light: label += "╱"; break;
                case MenstruationFlow.Heavy: label += "╳"; break;
            }
            return label.Trim();
        }
    }
}
