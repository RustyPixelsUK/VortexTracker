using System;
using VortexTracker;
using VortexTracker.Core.Actions;

namespace VTAvalonia.Services;

public sealed class AvaloniaUIActionDispatcher : IUIActionDispatcher
{
    public event EventHandler<UIActionType>? Executed;

    public void Execute(UIActionType actionType)
    {
        Executed?.Invoke(this, actionType);
    }
}
