using System;
namespace cocos2d.EmbeddableView.OpenTK
{
    /// <summary>
    /// Used to indicate how to calculate the count/length of a parameter.
    ///
    /// Only one of Parameter, Count, or Computed should be set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CountAttribute : Attribute
    {
        /// <summary>
        /// Specifies another parameter to look at for the count of this parameter.
        /// </summary>
        public string Parameter;

        /// <summary>
        /// Specifies a fixed count.
        /// </summary>
        public int Count;

        /// <summary>
        /// Specifies a computed count based on other parameters.
        /// </summary>
        public string Computed;

        /// <summary>
        /// Constructs a new CountAttribute instance.
        /// </summary>
        public CountAttribute()
        {
        }
    }
}

