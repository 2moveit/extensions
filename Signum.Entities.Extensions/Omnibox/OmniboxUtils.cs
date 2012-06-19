﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;

namespace Signum.Entities.Omnibox
{
    public static class OmniboxUtils
    {
        public static bool IsPascalCasePattern(string ident)
        {
            if (string.IsNullOrEmpty(ident))
                return false;

            for (int i = 0; i < ident.Length; i++)
            {
                if (!char.IsUpper(ident[i]))
                    return false;
            }

            return true;
        }




        public static OmniboxMatch SubsequencePascal(object value, string identifier, string pattern)
        {
            int[] indices = null;
            int j = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                var pc = pattern[i];
                for (; j < identifier.Length; j++)
                {
                    var ic = identifier[j];

                    if (char.IsUpper(ic))
                    {
                        if (ic == pc)
                        {
                            if (indices == null)
                                indices = new int[pattern.Length];

                            indices[i] = j;

                            break;
                        }
                    }
                }

                if (j == identifier.Length)
                    return null;

                j++;
            }

            return new OmniboxMatch(value,
                remaining: identifier.Count(char.IsUpper) - pattern.Length,
                choosenString: identifier,
                boldIndices: indices);
        }

        public static IEnumerable<OmniboxMatch> Matches<T>(Dictionary<string, T> values, Func<T, string> niceName, string pattern, bool isPascalCase)
        {
            T val;
            if (values.TryGetValue(pattern, out val))
            {
                yield return new OmniboxMatch(val, 0, pattern, 0.To(pattern.Length).ToArray());
            }
            else
            {
                foreach (var kvp in values)
                {
                    OmniboxMatch result;
                    if (isPascalCase)
                    {
                        result = SubsequencePascal(kvp.Value, kvp.Key, pattern);

                        if (result != null)
                        {
                            yield return result;
                            continue;
                        }
                    }

                    result = Contains(kvp.Value, kvp.Key, pattern);
                    if (result != null)
                    {
                        yield return result;
                        continue;
                    }

                    result = Contains(kvp.Value, niceName(kvp.Value), pattern);
                    if (result != null)
                    {
                        yield return result;
                        continue;
                    }
                }
            }
        }

        public static OmniboxMatch Contains(object value, string identifier, string pattern)
        {
            int index = identifier.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
                return null;

            return new OmniboxMatch(value,
                remaining: identifier.Length - pattern.Length,
                choosenString: identifier,
                boldIndices: 0.To(pattern.Length).Select(i => index + i).ToArray());
        }

        public static string CleanCommas(string str)
        {
            return str.Substring(1, str.Length - 2);
        }
    }

    public class OmniboxMatch
    {
        public OmniboxMatch(object value, int remaining, string choosenString, int[] boldIndices)
        {
            this.Value = value;

            this.Text = choosenString;
            this.BoldIndices = boldIndices;

            this.Distance = remaining;

            if (BoldIndices[0] == 0)
                this.Distance /= 2f;
        }

        public object Value; 

        public float Distance;
        public string Text;
        public int[] BoldIndices;

        public IEnumerable<Tuple<string, bool>> BoldSpans()
        {
            bool[] isBool = new bool[Text.Length];

            for (int i = 0; i < BoldIndices.Length; i++)
                isBool[BoldIndices[i]] = true;

            bool lastIsBool = isBool[0];
            int lastIndex = 0;
            for (int i = 1; i < Text.Length; i++)
            {
                if (isBool[i] != lastIsBool)
                {
                    yield return Tuple.Create(Text.Substring(lastIndex, i - lastIndex), lastIsBool);

                    lastIsBool = isBool[i];
                    lastIndex = i;
                }
            }

            yield return Tuple.Create(Text.Substring(lastIndex, Text.Length - lastIndex), lastIsBool);
        }
    }
}
