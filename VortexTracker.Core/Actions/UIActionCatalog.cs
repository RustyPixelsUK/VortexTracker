namespace VortexTracker.Core.Actions;

public sealed class UIActionCatalog
{
    private readonly Dictionary<UIActionType, UIActionDefinition> _definitions = new();
    private readonly Dictionary<UIActionType, UIActionState> _states = new();

    public void Add(UIActionDefinition definition)
    {
        _definitions[definition.ActionType] = definition;
        if (!_states.ContainsKey(definition.ActionType))
            _states[definition.ActionType] = new UIActionState();
    }

    public UIActionDefinition? GetDefinition(UIActionType actionType)
        => _definitions.TryGetValue(actionType, out var definition) ? definition : null;

    public UIActionState GetState(UIActionType actionType)
    {
        if (!_states.TryGetValue(actionType, out var state))
        {
            state = new UIActionState();
            _states[actionType] = state;
        }

        return state;
    }

    public IReadOnlyCollection<UIActionDefinition> Definitions => _definitions.Values;
}
