/*
Copyright 2011 - 2022 Adrian Popescu

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
using System.Diagnostics;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// the module name: boards, calendar, documents, files, gant, issue_tracking, news, repository, time_tracking, wiki.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.ENABLED_MODULE)]
    public sealed class ProjectEnabledModule : IdentifiableName, IValue
    {
        #region Ctors
        /// <summary>
        /// 
        /// </summary>
        public ProjectEnabledModule() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        public ProjectEnabledModule(string moduleName)
        {
            if (moduleName.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("The module name should be one of: boards, calendar, documents, files, gant, issue_tracking, news, repository, time_tracking, wiki.", nameof(moduleName));
            }

            Name = moduleName;
        }

        #endregion

        #region Implementation of IValue
        /// <summary>
        /// 
        /// </summary>
        public string Value => Name;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(ProjectEnabledModule)}: {ToString()}]";

    }
}