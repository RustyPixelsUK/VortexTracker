namespace VortexTracker.Core.Actions;

public interface IUIActionDispatcher
{
    event EventHandler<UIActionType>? Executed;
    void Execute(UIActionType actionType);
}
