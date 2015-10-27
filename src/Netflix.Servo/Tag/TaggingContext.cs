namespace Netflix.Servo.Tag
{
    /**
 * Returns the set of tags associated with the current execution context.
 * Implementations of this interface are used to provide a common set of tags
 * for all contextual monitors in a given execution flow.
 */
    public interface TaggingContext
    {
        /**
         * Returns the tags for the current execution context.
         */
        ITagList getTags();
    }
}
