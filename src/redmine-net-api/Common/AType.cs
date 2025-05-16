using System;

namespace Redmine.Net.Api.Common;

internal readonly struct A<T>{
    public static A<T> Is => default;
#pragma warning disable CS0184 // 'is' expression's given expression is never of the provided type
    public static bool IsEqual<U>() => Is is A<U>;
#pragma warning restore CS0184 // 'is' expression's given expression is never of the provided type
    public static Type Value => typeof(T);                         
                             
}