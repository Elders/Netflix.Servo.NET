using System;

namespace Netflix.Servo.Util
{
    /**
 * Internal convenience methods that help a method or constructor check whether it was invoked
 * correctly. Please notice that this should be considered an internal implementation detail, and
 * it is subject to change without notice.
 */
    public class Preconditions
    {
        private Preconditions()
        {
        }

        /**
         * Ensures the object reference is not null.
         */
        public static T checkNotNull<T>(T obj, string name)
        {
            if (ReferenceEquals(null, obj))
            {
                string msg = $"parameter '{name}' cannot be null";
                throw new NullReferenceException(msg);
            }
            return obj;
        }

        /**
         * Ensures the truth of an expression involving one or more parameters to the
         * calling method.
         *
         * @param expression a boolean expression
         * @throws IllegalArgumentException if {@code expression} is false
         */
        public static void checkArgument(bool expression, string errorMessage)
        {
            if (!expression)
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
