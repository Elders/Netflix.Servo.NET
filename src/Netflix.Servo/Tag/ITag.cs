namespace Netflix.Servo.Tag
{
    /// <summary>
    /// A key-value pair associated with a metric.
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Returns the key corresponding to this tag.
        /// </summary>
        /// <returns></returns>
        string getKey();

        /// <summary>
        /// Returns the value corresponding to this tag.
        /// </summary>
        /// <returns></returns>
        string getValue();

        /// <summary>
        /// Returns the string representation of this tag.
        /// </summary>
        /// <returns></returns>
        string tagString();
    }
}
