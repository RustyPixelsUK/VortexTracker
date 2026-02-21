using VortexTracker.Core.Actions;

namespace VortexTracker;

public static class WinFormsActionCatalogFactory
{
    public static (UIActionCatalog Catalog, IReadOnlyList<UIActionDefinition> Definitions) CreateDefault()
        => UIActionCatalogFactory.CreateDefault();
}
