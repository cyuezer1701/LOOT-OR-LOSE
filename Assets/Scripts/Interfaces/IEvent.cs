using LootOrLose.Enums;

namespace LootOrLose.Interfaces
{
    /// <summary>
    /// Defines the contract for in-run events such as Merchants, Altars, Chests, and more.
    /// Events provide opportunities for the player to interact with the dungeon beyond standard loot decisions.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The type of event, determining its behavior and visual presentation.
        /// </summary>
        EventType Type { get; }

        /// <summary>
        /// Localization key for the event's title displayed in the UI.
        /// </summary>
        string TitleKey { get; }

        /// <summary>
        /// Localization key for the event's description text explaining the encounter.
        /// </summary>
        string DescriptionKey { get; }

        /// <summary>
        /// Whether this event requires the player to sacrifice or use an inventory item to participate.
        /// </summary>
        bool RequiresItem { get; }

        /// <summary>
        /// Executes the event logic against the current run state and returns the outcome.
        /// </summary>
        /// <param name="runState">The current state of the player's run, including inventory and stats.</param>
        /// <returns>An <see cref="EventResult"/> describing what happened as a result of the event.</returns>
        EventResult Execute(GameRunState runState);
    }
}
