using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netlenium.Driver.WebDriver.Interactions;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Interface allowing execution of W3C Specification-compliant actions.
    /// </summary>
    public interface IActionExecutor
    {
        /// <summary>
        /// Gets a value indicating whether this object is a valid action executor.
        /// </summary>
        bool IsActionExecutor { get; }

        /// <summary>
        /// Performs the specified list of actions with this action executor.
        /// </summary>
        /// <param name="actionSequenceList">The list of action sequences to perform.</param>
        void PerformActions(IList<ActionSequence> actionSequenceList);

        /// <summary>
        /// Resets the input state of the action executor.
        /// </summary>
        void ResetInputState();
    }
}
