using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Game.Extensions {
    /// <summary>
    /// Extension methods for string.
    /// Sources: https://gist.github.com/omgwtfgames/f917ca28581761b8100f
    /// </summary>
    public static class IDictionaryExtensions {

        // Named format strings from object attributes. Eg:
        // string blaStr = aPerson.ToString("My name is {FirstName} {LastName}.")
        // From: http://www.hanselman.com/blog/CommentView.aspx?guid=fde45b51-9d12-46fd-b877-da6172fe1791

        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
    }
}