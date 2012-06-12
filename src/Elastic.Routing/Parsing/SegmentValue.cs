using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// The URL segment value.
    /// </summary>
    public sealed class SegmentValue
    {
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>
        /// The string value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the value is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; private set; }

        private SegmentValue()
        {
        }

        /// <summary>
        /// Creates the segment value.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="isDefault">if set to <c>true</c> the value is meant as default.</param>
        /// <returns>The new instance of the <see cref="SegmentValue"/> or <c>null</c> if the <paramref name="value"/> is <c>null</c>.</returns>
        public static SegmentValue Create(string value, bool isDefault = false)
        {
            if (value == null)
                return null;

            return new SegmentValue
            {
                Value = value,
                IsDefault = isDefault
            };
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Elastic.Routing.Parsing.SegmentValue"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator string(SegmentValue value)
        {
            return value != null ? value.Value : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="Elastic.Routing.Parsing.SegmentValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator SegmentValue(string value)
        {
            return Create(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
