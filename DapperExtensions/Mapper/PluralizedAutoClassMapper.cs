using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DapperExtensions.Mapper
{
    /// <summary>
    /// Automatically maps an entity to a table using a combination of reflection and naming conventions for keys. 
    /// Identical to AutoClassMapper, but attempts to pluralize table names automatically.
    /// Example: Person entity maps to People table
    /// </summary>
    public class PluralizedAutoClassMapper<T> : AutoClassMapper<T> where T : class
    {
        public override void Table(string tableName)
        {
            base.Table(Formatting.Pluralize(tableName));
        }
        
        // Adapted from: http://mattgrande.wordpress.com/2009/10/28/pluralization-helper-for-c/
        public static class Formatting
        {
            private static readonly IList<string> s_unpluralizables = new List<string> { "equipment", "information", "rice", "money", "species", "series", "fish", "sheep", "deer" };
            private static readonly IDictionary<string, string> s_pluralizations = new Dictionary<string, string>
                                                                                     {
                                                                                         // Start with the rarest cases, and move to the most common
                                                                                         { "person", "people" },
                                                                                         { "ox", "oxen" },
                                                                                         { "child", "children" },
                                                                                         { "foot", "feet" },
                                                                                         { "tooth", "teeth" },
                                                                                         { "goose", "geese" },
                                                                                         // And now the more standard rules.
                                                                                         { "(.*)fe?$", "$1ves" }, // ie, wolf, wife
                                                                                         { "(.*)man$", "$1men" },
                                                                                         { "(.+[aeiou]y)$", "$1s" },
                                                                                         { "(.+[^aeiou])y$", "$1ies" },
                                                                                         { "(.+z)$", "$1zes" },
                                                                                         { "([m|l])ouse$", "$1ice" },
                                                                                         { "(.+)(e|i)x$", @"$1ices"}, // ie, Matrix, Index
                                                                                         { "(octop|vir)us$", "$1i"},
                                                                                         { "(.+(s|x|sh|ch))$", @"$1es"},
                                                                                         { "(.+)", @"$1s" }
                                                                                     };

            public static string Pluralize(string singular)
            {
                if (s_unpluralizables.Contains(singular))
                {
	                return singular;
                }

                string plural = string.Empty;

                foreach (var pluralization in s_pluralizations)
                {
                    if (Regex.IsMatch(singular, pluralization.Key))
                    {
                        plural = Regex.Replace(singular, pluralization.Key, pluralization.Value);
                        break;
                    }
                }

                return plural;
            }
        }
    }
}