using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryMining
{
    public struct Regexes
    {
        public static List<string> StatsPatterns
        {
            get
            {
                return new string[] {
                    Cost, GP, NetProfit, ROI, NPPerConv, GPPerConv, Conversions, Clicks, Impressions, ConvValPerCost, CTR, AvgCPC,
                    AvgPosition, CostPerConv, ConvRate, ViewThroughConv, QueryCount }.ToList();
            }
        }
        public static List<string> AllPatterns
        {
            get
            {
                return new string[]
                { Word, Query, Cost, GP, NetProfit, ROI, NPPerConv, GPPerConv, Conversions, Clicks, Impressions, ConvValPerCost, CTR, AvgCPC, AvgPosition, CostPerConv, ConvRate, ViewThroughConv, Number, Average, Percent, QueryCount}
                .ToList();
            }
        }

        private const RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline;
        public const string Word = @"Word";
        public const string Query = @"^Query$|(Search ?term)";
        public const string Cost = @"Cost";
        public const string GP = @"(GP)|(Gross ?Profit)|(Total ?(Conv\.?|Conversion) (value|val\.?))";
        public const string NetProfit = @"Net\s?Profit";
        public const string ROI = @"ROI|ROAS";
        public const string NPPerConv = @"NP ?\/ ?Conv|NPPerConv";
        public const string GPPerConv = @"GP ?\/ ?Conv\.?|GPPerConv";
        public const string Conversions = @"^Conv\.?$|Conversion(s)?";
        public const string Clicks = @"Clicks";
        public const string Impressions = @"Impressions|Imp\.?";
        public const string ConvValPerCost = @"Conv\.? ?value ?\/ ?cost|ConvValPerCost";
        public const string CTR = @"CTR|Clickthrough ?rate";
        public const string AvgCPC = @"Avg\.? ?CPC";
        public const string AvgPosition = @"Avg\.? ?Position";
        public const string CostPerConv = @"Cost\.? ?\/ ?Conv\.?|CostPerConv(ersion)?";
        public const string ConvRate = @"(Conv\.?|Conversion) ?Rate";
        public const string ViewThroughConv = @"View\-?through ?Conv\.?";
        public const string Number = @"^-?\d+(\.{1}\d*)?$";
        public const string QueryCount = @"^Query ?Count$";
        public const string Average = @"(Avg\.?)|(Average)|(\\|\/)|" + ROI + "|" + CTR + "|" + @"(.*Rate.*)|(.*\%.*)";
        public const string Percent = @".*\%.*";

        public static List<string> DecimalPatterns
        {
            get
            {
                if (Program.AvgAll)
                    return StatsPatterns;
                else
                    return new string[] { Cost, GP, NetProfit, NPPerConv, GPPerConv, ConvValPerCost, AvgCPC, CostPerConv }.ToList();
            }
        }
        public static List<string> DoublePatterns
        {
            get
            {
                if (Program.AvgAll)
                    return new List<string>();
                else
                    return new string[] { ROI, CTR, AvgPosition, ConvRate, ViewThroughConv }.ToList();
            }
        }
        public static List<string> IntPatterns
        {
            get
            {
                if (Program.AvgAll)
                    return new List<string>();
                else
                    return new string[] { Conversions, Clicks }.ToList();
            }
        }
        public static List<string> LongPatterns
        {
            get
            {
                if (Program.AvgAll)
                    return new List<string>();
                else
                    return new string[] { Impressions, }.ToList();
            }
        }

        public static List<string> ExcludedWords
        {
            get { return new string[] { "a", "an", "and", "at", "the", "of", "for" }.ToList(); }
        }
        public static string Match(string target, string pattern)
        {
            return Regex.Match(target, pattern, _options).ToString();
        }
        public static bool IsMatch(string target, string pattern, RegexOptions options = _options)
        {
            try
            {
                return Regex.IsMatch(target, pattern, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static bool MatchesAnyStat(string target)
        {
            int matches = (from expr in StatsPatterns
                           where Regexes.IsMatch(target, expr)
                           select true).Count();

            return matches > 0;
        }
        public static bool IsDecimal(string target)
        {
            List<bool> matches = (from expr in DecimalPatterns
                                  select Regexes.IsMatch(target, expr)).ToList();
            return matches.Any(match => match);
        }
        public static bool IsDouble(string target)
        {
            List<bool> matches = (from expr in DoublePatterns
                                  select Regexes.IsMatch(target, expr)).ToList();
            return matches.Any(match => match);
        }
        public static bool IsInt(string target)
        {
            var matches = (from expr in IntPatterns
                           select Regexes.IsMatch(target, expr)).Count();
            return matches > 0;
        }

        public static bool IsNumber(String target, RegexOptions options = _options)
        {
            return IsMatch(target, Regexes.Number, options);
        }

        public static bool IsLong(string target)
        {
            List<bool> matches = (from expr in LongPatterns
                                  select Regexes.IsMatch(target, expr)).ToList();
            return matches.Any(match => match);
        }

        public static bool IsExcluded(string target, RegexOptions options = _options)
        {
            if (target.Length <= 2 || Regexes.IsNumber(target, options))
                return true;

            int matches = (from expr in ExcludedWords
                           where target == expr
                           select true).Count();

            return matches > 0;
        }
    }
}
