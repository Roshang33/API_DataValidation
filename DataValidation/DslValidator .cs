using System.Reflection;

namespace DataValidation
{
    public class DslValidator
    {
        private readonly Dictionary<string, Func<string, Task<bool>>> _validationFunctionMap;

        public DslValidator()
        {
            _validationFunctionMap = new Dictionary<string, Func<string, Task<bool>>>(StringComparer.OrdinalIgnoreCase);
            LoadPluginsAsync().Wait(); // Load plugins asynchronously
        }

        private async Task LoadPluginsAsync()
        {
            string pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (!Directory.Exists(pluginDirectory))
                return;

            var pluginFiles = Directory.GetFiles(pluginDirectory, "*.dll");

            await Task.WhenAll(pluginFiles.Select(async file =>
            {
                try
                {
                    Assembly assembly = await Task.Run(() => Assembly.LoadFrom(file));
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(IValidationPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    await Task.WhenAll(pluginTypes.Select(async type =>
                    {
                        var pluginInstance = (IValidationPlugin)Activator.CreateInstance(type);
                        var functions = pluginInstance.GetValidationFunctions();

                        lock (_validationFunctionMap) // Thread-safe update
                        {
                            foreach (var kvp in functions)
                            {
                                _validationFunctionMap.TryAdd(kvp.Key, value => Task.FromResult(kvp.Value(value)));
                            
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin from {file}: {ex.Message}");
                }
            }));
        }

        public async Task<bool> ExecuteCustomValidationAsync(string functionName, string value)
        {
            if (_validationFunctionMap.TryGetValue(functionName, out var function))
            {
                return await function(value);
            }

            Console.WriteLine($"Error: Validation function '{functionName}' not found.");
            return false;
        }
    }
}