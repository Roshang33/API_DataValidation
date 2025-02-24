using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataValidation.Helper
{
    public static class Helper
    {
        public static Dictionary<string, string> MapValues(JObject ruleTemplate, JObject payload)
        {
            var result = new Dictionary<string, string>();
            var stack = new Stack<(JObject ruleNode, JObject payloadNode)>();

            stack.Push((ruleTemplate, payload));

            while (stack.Count > 0)
            {
                var (ruleNode, payloadNode) = stack.Pop();

                foreach (var property in ruleNode.Properties())
                {
                    if (property.Value.Type == JTokenType.Object)
                    {
                        // If the payload has the corresponding nested object, push it onto the stack
                        if (payloadNode.TryGetValue(property.Name, out JToken payloadValue) && payloadValue.Type == JTokenType.Object)
                        {
                            stack.Push(((JObject)property.Value, (JObject)payloadValue));
                        }
                    }
                    else
                    {
                        // Map function name to corresponding payload value
                        if (payloadNode.TryGetValue(property.Name, out JToken value))
                        {
                            result[property.Value.ToString()] = value.ToString();
                        }
                    }
                }
            }

            return result;
        }
    }
}
