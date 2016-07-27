namespace Elders.Servo.NET.Tag
{
    /// <summary>
    /// A key-value pair associated with a metric.
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Returns the key corresponding to this tag.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Returns the value corresponding to this tag.
        /// </summary>
        string Value { get; }
    }
}
