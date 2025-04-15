﻿/*
   Copyright 2011 - 2025 Adrian Popescu

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

using System.Diagnostics;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.TIME_ENTRY_ACTIVITY)]
    public sealed class ProjectTimeEntryActivity : IdentifiableName
    {
        /// <summary>
        /// 
        /// </summary>
        public ProjectTimeEntryActivity() { }

        internal ProjectTimeEntryActivity(int id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(ProjectTimeEntryActivity)}: {ToString()}]";

    }
}