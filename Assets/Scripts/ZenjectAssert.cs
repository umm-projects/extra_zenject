using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ExtraZenject
{
    // In order to avoid confusion with UnityEngine.Assertion and ModestTree, class names are dared to be redundant
    public static class ZenjectAssert
    {
        [Conditional("UNITY_EDITOR")]
        public static void IsInjected<T>(T value, string message = "")
        {
            if (value == null)
            {
                Fail(string.IsNullOrEmpty(message) ? $"`{typeof(T)}' seems to be not injected." : message, true);
            }
        }

        [Conditional("UNITY_EDITOR")]
        private static void Fail(string message, bool useWarning = false)
        {
            if (useWarning)
            {
                Debug.LogWarning($"Assertion warning: {message}");
            }
            else
            {
                Debug.LogAssertion($"Assertion failed: {message}");
            }
        }
    }
}