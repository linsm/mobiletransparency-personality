public class ApiKeyService : IApiKeyService
{
    private ApiKey treeManagerKey;
    private ApiKey logWriterKey;

    public ApiKeyService() {
        // load api keys from environment variables
        string? keyTreeManager = Environment.GetEnvironmentVariable("key_treemanager");
        string? keyLogWriter = Environment.GetEnvironmentVariable("key_logwriter");

        if(keyTreeManager == null) {
            throw new ArgumentNullException("key_treemanager"); 
        }
        if(keyLogWriter == null) {
            throw new ArgumentNullException("key_logwriter"); 
        }
        
        treeManagerKey = new ApiKey(keyTreeManager, new List<Role> { Role.TreeManager });
        logWriterKey = new ApiKey(keyLogWriter, new List<Role> { Role.LogWriter });
    }

    public ApiKey? VerifyApiKey(string key)
    {           
        if(key == treeManagerKey.key) {
            return treeManagerKey;
        }
        if(key == logWriterKey.key) {
            return logWriterKey;
        }                
        return null;
    }
}