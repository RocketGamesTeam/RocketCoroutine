using System;

namespace RocketCoroutine
{
    public static class ActionExtensions
    {
        public static void DoAfterFixedUpdate(this Action action, string key = null)
        {
			//test
            CoroutineController.DoAfterFixedUpdate(action, key);
        }

        public static void DoAfterCondition(this Action action, Func<bool> predicate, string key = null)
        {
            CoroutineController.DoAfterCondition(predicate, action, key);
        }

        public static void DoAfterGivenTime(this Action action, float time, string key = null)
        {
            CoroutineController.DoAfterGivenTime(time, action, key);
        }
    }
}
