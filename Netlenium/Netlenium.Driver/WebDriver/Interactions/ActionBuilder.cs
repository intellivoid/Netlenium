using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netlenium.Driver.WebDriver.Interactions
{
    /// <summary>
    /// Provides methods that allow the creation of action sequences to enable
    /// advanced user interactions.
    /// </summary>
    public class ActionBuilder
    {
        private Dictionary<InputDevice, ActionSequence> sequences = new Dictionary<InputDevice, ActionSequence>();

        /// <summary>
        /// Adds an action to the built set of actions. Adding an action will
        /// add a "tick" to the set of all actions to be executed.
        /// </summary>
        /// <param name="actionToAdd">The action to add to the set of actions</param>
        /// <returns>A self reference.</returns>
        public ActionBuilder AddAction(Interaction actionToAdd)
        {
            AddActions(actionToAdd);
            return this;
        }

        /// <summary>
        /// Adds an action to the built set of actions. Adding an action will
        /// add a "tick" to the set of all actions to be executed. Only one action
        /// for each input device may be added for a single tick.
        /// </summary>
        /// <param name="actionsToAdd">The set actions to add to the existing set of actions.</param>
        /// <returns>A self reference.</returns>
        public ActionBuilder AddActions(params Interaction[] actionsToAdd)
        {
            ProcessTick(actionsToAdd);
            return this;
        }

        /// <summary>
        /// Converts the set of actions in this <see cref="ActionBuilder"/> to a <see cref="List{ActionSequence}"/>.
        /// </summary>
        /// <returns>A <see cref="IList{ActionSequence}"/> suitable for transmission across the wire.
        /// The collection returned is read-only.</returns>
        public IList<ActionSequence> ToActionSequenceList()
        {
            return new List<ActionSequence>(sequences.Values).AsReadOnly();
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ActionBuilder"/>.
        /// </summary>
        /// <returns>A string that represents the current <see cref="ActionBuilder"/>.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var sequence in sequences.Values)
            {
                builder.AppendLine(sequence.ToString());
            }

            return builder.ToString();
        }

        private void ProcessTick(params Interaction[] interactionsToAdd)
        {
            var usedDevices = new List<InputDevice>();
            foreach (var interaction in interactionsToAdd)
            {
                var actionDevice = interaction.SourceDevice;
                if (usedDevices.Contains(actionDevice))
                {
                    throw new ArgumentException("You can only add one action per device for a single tick.");
                }
            }

            var unusedDevices = new List<InputDevice>(sequences.Keys);
            foreach (var interaction in interactionsToAdd)
            {
                var sequence = FindSequence(interaction.SourceDevice);
                sequence.AddAction(interaction);
                unusedDevices.Remove(interaction.SourceDevice);
            }

            foreach (var unusedDevice in unusedDevices)
            {
                var sequence = sequences[unusedDevice];
                sequence.AddAction(new PauseInteraction(unusedDevice, TimeSpan.Zero));
            }
        }

        private ActionSequence FindSequence(InputDevice device)
        {
            if (sequences.ContainsKey(device))
            {
                return sequences[device];
            }

            var longestSequenceLength = 0;
            foreach (var pair in sequences)
            {
                longestSequenceLength = Math.Max(longestSequenceLength, pair.Value.Count);
            }

            var sequence = new ActionSequence(device, longestSequenceLength);
            sequences[device] = sequence;

            return sequence;
        }
    }
}
