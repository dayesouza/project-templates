namespace TemplateIHD.CrossCutting.Interfaces
{
    public interface IAzureKeyVaultService
    {
        string GetSecret(string key);
    }
}
