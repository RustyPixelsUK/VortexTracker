using VortexTracker.Core.Actions;

namespace VortexTracker;

public sealed class WinFormsUIActionDispatcher : IUIActionDispatcher
{
    public event EventHandler<UIActionType>? Executed;

    public WinFormsUIActionDispatcher()
    {
        UIActionManager.Instance.ActionExecuted += (_, action) => Executed?.Invoke(this, action);
    }

    public void Execute(UIActionType actionType)
    {
        if (Globals.MainForm != null)
            UIActionManager.Instance.Execute(Globals.MainForm, actionType);
        else
            UIActionManager.Instance.Execute(this, actionType);
    }
}
