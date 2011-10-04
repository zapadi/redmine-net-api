/*
   Copyright 2011 Dorin Huzum, Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("time_entry")]
    public class TimeEntry
    {
        /// <summary>
        /// Gets or sets the issue id.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlAttribute("issue_id")]
        public int IssueId { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlAttribute("project_id")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the spent on.
        /// </summary>
        /// <value>The spent on.</value>
        [XmlAttribute("spent_on")]
        public DateTime SpentOn { get; set; }

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        /// <value>The hours.</value>
        [XmlAttribute("hours")]
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        /// <value>The activity id.</value>
        [XmlAttribute("activity_id")]
        public int ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        [XmlAttribute("comments")]
        public String Comments { get; set; }
    }
}