namespace VortexTracker.Core.Services;

public interface IRegistryService
{
    void SetFileAssociation(string extension, string progId, string description, string applicationPath);
}
