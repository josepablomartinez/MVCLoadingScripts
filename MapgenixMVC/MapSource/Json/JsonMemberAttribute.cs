using System;

namespace Mapgenix.GSuite.Mvc
{
    public sealed class JsonMemberAttribute : Attribute
    {
        public string MemberName { get; set; }
        public string ValuePropertyName { get; set; }
    }
}
