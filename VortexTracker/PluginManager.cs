using LibVT;
using LibVT.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VortexTracker
{
    public static class PluginManager
    {
        public const int MaxPlugins = 32;

        public static List<PluginWrapper> PluginWrappers { get; } = new();
        public static IEnumerable<IPlugin> Plugins =>
            PluginWrappers.Where(w => w.Instance != null).Select(w => w.Instance);

        static PluginManager()
        {
            string pluginsDir = Path.Combine(MainForm.VortexDocumentsDir, MainForm.PluginsDefaultDir);

            DiscoverPlugins(pluginsDir, new string[] { "libVT" });
            LoadEnabledPlugins(Globals.MainForm.PluginHost);
        }

        public static void DiscoverPlugins(string pluginsDir, string[] sharedLibraries)
        {
            PluginWrappers.Clear();                    // fresh scan each time
            IniFile ini = new IniFile(MainForm.ConfigFilePath);

            foreach (string dll in Directory.EnumerateFiles(pluginsDir, "*.dll",
                                                            SearchOption.AllDirectories))
            {
                if (!IsPluginAssembly(dll))            // skip non-plugins early
                    continue;

                string name = Path.GetFileNameWithoutExtension(dll);
                bool enabled = ReadEnabledFromIni(ini, name);

                PluginWrappers.Add(new PluginWrapper
                {
                    Name = name,
                    Path = dll,
                    IsEnabled = enabled,
                    Instance = null,                   // not loaded yet
                    Context = null,
                    SharedLibs = sharedLibraries
                });

                if (PluginWrappers.Count >= MaxPlugins)
                    break;                             // honour hard limit
            }
        }

        private static readonly string IPluginFullName = typeof(IPlugin).FullName!;

        private static bool IsPluginAssembly(string dllPath)
        {
            try
            {
                var resolverPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // 1) every assembly already loaded into the default ALC
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location))
                        resolverPaths.Add(asm.Location);
                }

                // 2) everything in the candidate’s own folder (private deps)
                string pluginDir = Path.GetDirectoryName(dllPath)!;
                foreach (string dep in Directory.EnumerateFiles(pluginDir, "*.dll"))
                    resolverPaths.Add(dep);

                // 3) the candidate itself
                resolverPaths.Add(dllPath);

                using var mlc = new MetadataLoadContext(
                        new PathAssemblyResolver(resolverPaths));

                var assembly = mlc.LoadFromAssemblyPath(dllPath);

                return assembly.GetTypes().Any(t =>
                    t.GetInterfaces().Any(i => i.FullName == IPluginFullName));
            }
            catch
            {
                // bad/mixed-mode/native DLLs will land here – treat them as "not a plugin"
                return false;
            }
        }

        private static bool ReadEnabledFromIni(IniFile ini, string pluginName)
        {
            // INI layout:  [Plugin1] Name=Foo  Enabled=true ...
            for (int i = 0; i < MaxPlugins; i++)
            {
                string section = $"Plugin{i + 1}";
                if (ini.GetValue(section, "Name")?.Equals(pluginName,
                             StringComparison.OrdinalIgnoreCase) == true)
                {
                    return ini.GetValue(section, "Enabled", false);
                }
            }
            return false;
        }

        public static void LoadEnabledPlugins(IHost host)
        {
            foreach (var w in PluginWrappers.Where(x => x.IsEnabled && x.Instance == null))
            {
                if (!TryActivatePlugin(w, host))
                    w.IsEnabled = false;
            }
        }

        public static bool TryActivatePlugin(PluginWrapper wrapper, IHost host)
        {
            try
            {
                var pluginLoadContext = new PluginLoadContext(wrapper.Path, wrapper.SharedLibs);
                Assembly assembly = pluginLoadContext.LoadFromAssemblyPath(wrapper.Path);

                // by convention VTPlugin.Plugin
                var type = assembly.GetType("VTPlugin.Plugin");
                if (type == null || !typeof(IPlugin).IsAssignableFrom(type))
                    return false;

                var plugin = (IPlugin)Activator.CreateInstance(type)!;
                plugin.Initialize(host);

                wrapper.Context = pluginLoadContext;
                wrapper.Instance = plugin;
                return true;
            }
            catch (Exception ex)
            {
                wrapper.LastError = ex.InnerException?.Message;
                wrapper.Context?.Unload();
                return false;
            }
        }

        public static bool UnloadPlugin(string pluginName)
            => TryGetWrapper(pluginName, out var w) && Unload(w);

        public static bool UnloadPlugin(IPlugin plugin)
            => TryGetWrapper(plugin, out var w) && Unload(w);

        private static bool Unload(PluginWrapper w)
        {
            try
            {
                w.Instance?.Dispose();
                w.Context?.Unload();
                w.Instance = null;
                w.Context = null;
                return true;
            }
            catch { return false; }
        }

        public static bool ReloadPlugin(string pluginName, IHost host)
            => TryGetWrapper(pluginName, out var w) && Reload(w, host);

        public static bool ReloadPlugin(IPlugin plugin, IHost host)
            => TryGetWrapper(plugin, out var w) && Reload(w, host);

        private static bool Reload(PluginWrapper w, IHost host)
        {
            if (!Unload(w)) return false;
            return w.IsEnabled && TryActivatePlugin(w, host);
        }

        private static bool TryGetWrapper(string name, out PluginWrapper wrapper)
        {
            wrapper = PluginWrappers.FirstOrDefault(
                          w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return wrapper != null;
        }

        private static bool TryGetWrapper(IPlugin plugin, out PluginWrapper wrapper)
        {
            wrapper = PluginWrappers.FirstOrDefault(w => w.Instance == plugin);
            return wrapper != null;
        }

        public static void RaiseAppEvent(object sender, AppEventArgs e)
            => foreachPlugin(p => p.OnAppEvent(sender, e));
        public static void RaiseUIEvent(object sender, UIEventArgs e)
            => foreachPlugin(p => p.OnUIEvent(sender, e));
        public static void RaiseRegisterEvent(object sender, RegisterEventArgs e)
            => foreachPlugin(p => p.OnRegisterEvent(sender, e));
        public static void RaisePlaybackEvent(object sender, PlaybackEventArgs e)
            => foreachPlugin(p => p.OnPlaybackEvent(sender, e));
        public static void RaiseMidiMessageEvent(object sender, MidiMessageEventArgs e)
            => foreachPlugin(p => p.OnMidiMessageEvent(sender, e));

        private static void foreachPlugin(Action<IPlugin> action)
        {
            foreach (var p in Plugins)
                action(p);
        }

        public static (string Version, string Author, string Description) ReadVersionResource(string dllPath)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(dllPath);
                string ver = info.ProductVersion ?? "";
                string auth = info.CompanyName ?? "";
                string descr = info.FileDescription ?? info.Comments ?? "";
                return (ver, auth, descr);
            }
            catch
            {
                return ("", "", "");
            }
        }
    }

    public sealed class PluginWrapper
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsEnabled { get; set; }
        public string[] SharedLibs { get; set; }
        public string LastError { get; set; }

        public IPlugin? Instance { get; set; }   // null until activated
        public PluginLoadContext? Context { get; set; }  // idem
    }

    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly HashSet<string> _shared;

        public PluginLoadContext(string pluginPath, IEnumerable<string> shared)
            : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            _shared = new HashSet<string>(shared, StringComparer.OrdinalIgnoreCase);
        }

        protected override Assembly Load(AssemblyName name)
        {
            if (_shared.Contains(name.Name))
                return null;                        // use Default ALC

            string path = _resolver.ResolveAssemblyToPath(name);
            return path != null ? LoadFromAssemblyPath(path) : null;
        }
    }
}