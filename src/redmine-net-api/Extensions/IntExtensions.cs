/*
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

namespace Redmine.Net.Api.Extensions;

internal static class IntExtensions
{
    public static bool Between(this int val, int from, int to)
    {
        return val >= from && val <= to;
    }
    
    public static bool Greater(this int val, int than)
    {
        return val > than;
    }
    
    public static bool GreaterOrEqual(this int val, int than)
    {
        return val >= than;
    }
    
    public static bool Lower(this int val, int than)
    {
        return val < than;
    }
    
    public static bool LowerOrEqual(this int val, int than)
    {
        return val <= than;
    }
}