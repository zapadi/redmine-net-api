/*
Copyright 2011 - 2015 Adrian Popescu, Dorin Huzum.

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
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.ENABLED_MODULE)]
    public class ProjectEnabledModule : IdentifiableName, IEquatable<ProjectEnabledModule>
    {
        /// <summary>
        /// the module name: boards, calendar, documents, files, gantt, issue_tracking, news, repository, time_tracking, wiki.
        /// </summary>
        new public string Name { get; set; }

        public bool Equals(ProjectEnabledModule other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name;
        }
    }
}