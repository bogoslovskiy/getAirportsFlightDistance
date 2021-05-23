namespace getAirportsFlightDistance
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.Json.Serialization;

    public class FluentValidationJsonPropertyNameResolver
    {
        public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            var jsonPropertyNameAttribute = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>();

            if (jsonPropertyNameAttribute != null)
            {
                return jsonPropertyNameAttribute.Name;
            }

            return memberInfo.Name;
        }
    }
}