namespace Netlenium.Driver
{
    /// <summary>
    /// Driver Interface
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="targetBrowser"></param>
        void IDriver(Browser targetBrowser);

        /// <summary>
        /// The target browser that this driver is targetting
        /// </summary>
        Browser TargetBrowser { get; }

        /// <summary>
        /// Driver Actions
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// The current document that's current loaded into the view
        /// </summary>
        IDocument Document { get; }
       
    }
}
