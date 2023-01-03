public interface IApiKeyService 
{
    ApiKey? VerifyApiKey(string key);
}