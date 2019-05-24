using System.Collections.Generic;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Defines an action that consists of a list of other actions to be performed in the browser.
    /// </summary>
    internal class CompositeAction : IAction
    {
        private List<IAction> actionsList = new List<IAction>();

        /// <summary>
        /// Adds an action to the list of actions to be performed.
        /// </summary>
        /// <param name="action">An <see cref="IAction"/> to be appended to the
        /// list of actions to be performed.</param>
        /// <returns>A self reference.</returns>
        public CompositeAction AddAction(IAction action)
        {
            actionsList.Add(action);
            return this;
        }

        /// <summary>
        /// Performs the actions defined in this list of actions.
        /// </summary>
        public void Perform()
        {
            foreach (var action in actionsList)
            {
                action.Perform();
            }
        }
    }
}
