using FeatureHubSDK;

namespace FHHelper;

public interface IFeatureStateProvider
{
    bool IsEnabled(string featureKey);
}

public class FeatureStateProvider : IFeatureStateProvider
{
    private readonly EdgeFeatureHubConfig _config;

    public FeatureStateProvider(string url, string sdkKey)
    {
        var config = new EdgeFeatureHubConfig(url, sdkKey);
        
        config.Init().Wait();
        
        _config = config;
    }

    public bool IsEnabled(string featureKey)
    {
        try
        {
            return (bool)_config.Repository[featureKey].Value!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}