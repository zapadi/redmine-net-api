using System;

namespace Redmine.Net.Api.Types
{
    public class CustomField : IdentifiableName
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public String Value { get; set; }
    }
}
