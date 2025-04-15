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

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.USER)]
    public sealed class Watcher : Identifiable<Watcher>
        ,ICloneable<Watcher>
        ,IValue
    {
        #region Implementation of IValue 
        /// <summary>
        /// 
        /// </summary>
        public string Value => Id.ToString(CultureInfo.InvariantCulture);

        #endregion

        #region Implementation of ICloneable<T> 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new Watcher Clone(bool resetId)
        {
            if (resetId)
            {
                return new Watcher();
            }
            return new Watcher
            {
                Id = Id
            };
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(Watcher)}: {ToString()}]";
    }
}