using System;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RequiredComponentsAttribute : Attribute
    {
        public Type[] RequiredTypes { get; }

        public RequiredComponentsAttribute(params Type[] requiredTypes)
        {
            RequiredTypes = requiredTypes;
        }
    }
}