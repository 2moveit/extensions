﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Signum.Utilities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Reflection;

namespace Signum.Entities.Omnibox
{
    public static class OmniboxParser
    {
        static OmniboxManager manager;

        public static OmniboxManager Manager
        {
            get
            {
                if (manager == null) 
                    throw new InvalidOperationException("OmniboxParse.Manager is not set");
                return manager;
            }

            set { manager = value; }
        }

        public static List<IOmniboxResultGenerator> Generators = new List<IOmniboxResultGenerator>();

        static readonly Regex tokenizer = new Regex(
@"(?<space>\s+)|
(?<ident>[a-zA-Z_][a-zA-Z0-9_]*)|
(?<number>[+-]?\d+(\.\d+)?)|
(?<string>("".*?(""|$)|\'.*?(\'|$)))|
(?<dot>\.)|
(?<semicolon>;)|
(?<comparer>(==?|<=|>=|<|>|\^=|\$=|%=|\*=|\!=|\!\^=|\!\$=|\!%=|\!\*=))", 
  RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);


        public static List<OmniboxResult> Results(string omniboxQuery)
        {
            List<OmniboxToken> tokens = new List<OmniboxToken>();

            foreach (Match m in tokenizer.Matches(omniboxQuery))
            {
                AddTokens(tokens, m, "ident", OmniboxTokenType.Identifier);
                AddTokens(tokens, m, "dot", OmniboxTokenType.Dot);
                AddTokens(tokens, m, "semicolon", OmniboxTokenType.Semicolon);
                AddTokens(tokens, m, "comparer", OmniboxTokenType.Comparer);
                AddTokens(tokens, m, "number", OmniboxTokenType.Number);
                AddTokens(tokens, m, "string", OmniboxTokenType.String);
                AddTokens(tokens, m, "date", OmniboxTokenType.String);
            }

            tokens.Sort(a => a.Index);

            var tokenPattern = new string(tokens.Select(t => Char(t.Type)).ToArray());

            List<OmniboxResult> result = new List<OmniboxResult>();
            foreach (var generator in Generators)
            {
                result.AddRange(generator.GetResults(omniboxQuery, tokens, tokenPattern));
            }

            result.Sort(a => a.Distance);

            return result;
        }

        static void AddTokens(List<OmniboxToken> tokens, Match m, string groupName, OmniboxTokenType type)
        {
            var group = m.Groups[groupName];

            if (group.Success)
            {
                tokens.Add(new OmniboxToken(type, group.Index, group.Value));
            }
        }

        static char Char(OmniboxTokenType omniboxTokenType)
        {
            switch (omniboxTokenType)
            {
                case OmniboxTokenType.Identifier: return 'I';
                case OmniboxTokenType.Dot: return '.';
                case OmniboxTokenType.Semicolon: return ';';
                case OmniboxTokenType.Comparer: return '=';
                case OmniboxTokenType.Number: return 'N';
                case OmniboxTokenType.String: return 'S';
                default: return '?';
            }
        }


    }

    public abstract class OmniboxManager
    {
        public abstract bool AllowedType(Type type);

        public abstract bool AllowedQuery(object queryName);
        public abstract QueryDescription GetDescription(object queryName);

        public abstract Lite RetrieveLite(Type type, int id);

        public abstract List<Lite> AutoComplete(Type cleanType, Implementations implementations, string subString, int count);

        internal string CleanQueryName(object queryName)
        {
            return (queryName is Type ? Reflector.CleanTypeName((Type)queryName) : queryName.ToString());
        }
    }

    public abstract class OmniboxResult
    {
        public float Distance;

    }

    public interface IOmniboxResultGenerator
    {
        IEnumerable<OmniboxResult> GetResults(string rawQuery, List<OmniboxToken> tokens, string tokenPattern); 
    }

    public abstract class OmniboxResultGenerator<T> : IOmniboxResultGenerator where T : OmniboxResult
    {
        public abstract IEnumerable<T> GetResults(string rawQuery, List<OmniboxToken> tokens, string tokenPattern);

        IEnumerable<OmniboxResult> IOmniboxResultGenerator.GetResults(string rawQuery, List<OmniboxToken> tokens, string tokenPattern)
        {
            return GetResults(rawQuery, tokens, tokenPattern);
        }
    }

    public struct OmniboxToken
    {
        public OmniboxToken(OmniboxTokenType type, int index, string value)
        {
            this.Type = type;
            this.Index = index;
            this.Value = value;
        }

        public readonly OmniboxTokenType Type;
        public readonly int Index;
        public readonly string Value;

        public bool IsNull()
        {
            if (Type == OmniboxTokenType.Identifier)
                return Value == "null" || Value == "none";

            if (Type == OmniboxTokenType.String)
                return Value == "\"\"";

            return false;
        }

        internal char? Next(string rawQuery)
        {
            int last = Index + Value.Length;

            if (last < rawQuery.Length)
                return rawQuery[last];

            return null;
        }
    }

    public enum OmniboxTokenType
    {
        Identifier,
        Dot,
        Semicolon,
        Comparer,
        Number,
        String,
    }
}
