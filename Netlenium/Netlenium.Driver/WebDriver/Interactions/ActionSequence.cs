using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Represents a sequence of actions to be performed in the target browser.
    /// </summary>
    public class ActionSequence
    {
        private List<Interaction> interactions = new List<Interaction>();
        private InputDevice device;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionSequence"/> class.
        /// </summary>
        /// <param name="device">The input device that executes this sequence of actions.</param>
        public ActionSequence(InputDevice device)
            : this(device, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionSequence"/> class.
        /// </summary>
        /// <param name="device">The input device that executes this sequence of actions.</param>
        /// <param name="initialSize">the initial size of the sequence.</param>
        public ActionSequence(InputDevice device, int initialSize)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device", "Input device cannot be null.");
            }

            this.device = device;

            for (var i = 0; i < initialSize; i++)
            {
                AddAction(new PauseInteraction(device, TimeSpan.Zero));
            }
        }

        /// <summary>
        /// Gets the count of actions in the sequence.
        /// </summary>
        public int Count
        {
            get { return interactions.Count; }
        }

        /// <summary>
        /// Adds an action to the sequence.
        /// </summary>
        /// <param name="interactionToAdd">The action to add to the sequence.</param>
        /// <returns>A self-reference to this sequence of actions.</returns>
        public ActionSequence AddAction(Interaction interactionToAdd)
        {
            if (interactionToAdd == null)
            {
                throw new ArgumentNullException("interactionToAdd", "Interaction to add to sequence must not be null");
            }

            if (!interactionToAdd.IsValidFor(device.DeviceKind))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Interaction {0} is invalid for device type {1}.", interactionToAdd.GetType(), device.DeviceKind), "interactionToAdd");
            }

            interactions.Add(interactionToAdd);
            return this;
        }

        /// <summary>
        /// Converts this action sequence into an object suitable for serializing across the wire.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the actions in this sequence.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            var toReturn = device.ToDictionary();

            var encodedActions = new List<object>();
            foreach (var action in interactions)
            {
                encodedActions.Add(action.ToDictionary());
            }

            toReturn["actions"] = encodedActions;

            return toReturn;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ActionSequence"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="ActionSequence"/>.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder("Action sequence - ").Append(device.ToString());
            foreach (var action in interactions)
            {
                builder.AppendLine();
                builder.AppendFormat("    {0}", action.ToString());
            }

            return builder.ToString();
        }
    }
}
