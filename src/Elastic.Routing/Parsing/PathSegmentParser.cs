using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// A path segments parser.
    /// </summary>
    public class PathSegmentParser
    {
        /// <summary>
        /// A singleton instance of the parser for performance optimizations.
        /// </summary>
        public static PathSegmentParser Instance = new PathSegmentParser();

        /// <summary>
        /// A regex instance which matches path parameters.
        /// </summary>
        protected Regex parameterRegex = new Regex(@"\{(?<wc>\*)?(?<n>\w[^\}]*)\}", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegmentParser"/> class.
        /// </summary>
        protected PathSegmentParser()
        {
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input path.</param>
        /// <param name="constraints">The parameter constraints.</param>
        /// <returns>Returns the tree of path segments.</returns>
        public virtual FullPathSegment Parse(string input, IDictionary<string, object> constraints)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var preList = ParseOptionals(input);
            var parameters = new HashSet<string>();
            var expandedList = Expand(preList, constraints, parameters);
            var result = new FullPathSegment(expandedList, parameters);
            return result;
        }

        /// <summary>
        /// Expands the specified segment collection which contains only optinal and literal segments.
        /// The optional segments are processed recursively and the literals are parsed for parameters.
        /// </summary>
        /// <param name="collection">The collection of segments to expand.</param>
        /// <param name="constraints">The constraints.</param>
        /// <param name="parameters">The parameters hash set to track the uniqueness.</param>
        /// <returns>Returns the expanded path segments tree.</returns>
        protected virtual PathSegmentCollection Expand(PathSegmentCollection collection, IDictionary<string, object> constraints, HashSet<string> parameters)
        {
            var result = new PathSegmentCollection();
            foreach (var item in collection)
            {
                if (item is LiteralPathSegment)
                {
                    var staticItem = (LiteralPathSegment)item;
                    var list = Parse(staticItem.Text, constraints, parameters);
                    result.AddRange(list);
                }
                else if (item is OptionalPathSegment)
                {
                    var optItem = new OptionalPathSegment();
                    result.Add(optItem);
                    var inner = Expand(item.Segments, constraints, parameters);
                    optItem.Segments.AddRange(inner);
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the input for optionals. The resulting collection contains only literal and optinal segments which are expanded into the full tree later by the <see cref="M:Expand"/> method.
        /// </summary>
        /// <param name="input">The input path.</param>
        /// <returns>Returns the collection of literal and optional segments.</returns>
        protected virtual PathSegmentCollection ParseOptionals(string input)
        {
            Action<StringBuilder, Stack<PathSegmentCollection>> flush = (b,s) =>
            {
                if (b.Length > 0)
                {
                    s.Peek().Add(new LiteralPathSegment(b.ToString()));
                    b.Length = 0;
                }
            };
            var sb = new StringBuilder();
            var stack = new Stack<PathSegmentCollection>();
            stack.Push(new PathSegmentCollection());
            for (int i = 0; i < input.Length; i++)
            {
                var ch = input[i];
                if (ch == '(')
                {
                    flush(sb, stack);
                    var item = new OptionalPathSegment();
                    stack.Peek().Add(item);
                    stack.Push(item.Segments);
                }
                else if (ch == ')')
                {
                    if (stack.Count <= 1)
                        throw new FormatException("The number of closing braces is greater than the number of opening.");
                    flush(sb, stack);
                    stack.Pop();
                }
                else
                {
                    sb.Append(ch);
                }
            }
            if (stack.Count > 1)
                throw new FormatException(String.Format("Missing {0} closing brace(s).", stack.Count - 1));

            flush(sb, stack);
            return stack.Peek();
        }

        /// <summary>
        /// Parses the specified input converting it into the list of path segments.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="constraints">The parameter constraints.</param>
        /// <param name="parameters">The hash set to track the uniqueness of the parameters.</param>
        /// <returns>Returns the list of path segments.</returns>
        protected virtual IList<PathSegment> Parse(string input, IDictionary<string, object> constraints, HashSet<string> parameters)
        {
            var result = new List<PathSegment>();
            if (String.IsNullOrWhiteSpace(input))
                return result;

            var matches = parameterRegex.Matches(input);
            int currentIndex = 0;
            foreach (Match match in matches)
            {
                if (match.Index > currentIndex)
                {
                    var value = input.Substring(currentIndex, match.Index - currentIndex);
                    result.Add(new LiteralPathSegment(value));
                }

                var name = match.Groups["n"].Value;
                var customPattern = constraints != null ? constraints[name] as string : null;

                if (parameters.Contains(name))
                    throw new FormatException("Duplicate parameter: " + name);
                else
                    parameters.Add(name);

                if (match.Groups["wc"].Success)
                    result.Add(new WildcardPathSegment(name, customPattern));
                else
                    result.Add(new ParameterPathSegment(name, customPattern));

                currentIndex = match.Index + match.Length;
            }
            if (currentIndex < input.Length)
            {
                var value = input.Substring(currentIndex);
                result.Add(new LiteralPathSegment(value));
            }
            return result;
        }
    }
}
