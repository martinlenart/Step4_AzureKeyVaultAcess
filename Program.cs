using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Step4_AzureKeyVaultAcess
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a new secret client using the default credential from Azure.Identity.
            // Set the environment variables AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.

            //The secret value created when registering the application (Client Secret) 
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "d4g8Q~JHsgEazgThY-_b8LjUiqRosP3j5wh2_bGJ"); 

             //This application as registered in Azure, i.e. ProgramID (ClientID)
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", "a5a73c04-a537-48d1-afad-622286be6ead");                

            //Subscription ID shown at the Key Vault 
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "9263a307-713e-4ffc-850a-968a678b7229");

            //Connect to the Azure KV using above credentials
            var keyVaultName = "webamp-kv-demo";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential(
                new DefaultAzureCredentialOptions { AdditionallyAllowedTenants = { "*" }}));

            //The secret
            string secretName = $"super-secret-{Guid.NewGuid()}";
            var secretValue = "hello from console app AzureKeyVaultAccess";

            //CRD on the secret 
            Console.Write($"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);
            Console.WriteLine(" done.");

            Console.WriteLine("Forgetting your secret.");
            secretValue = string.Empty;
            Console.WriteLine($"Your secret is '{secretValue}'.");

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

            Console.Write($"Deleting your secret from {keyVaultName} ...");
            DeleteSecretOperation operation = await client.StartDeleteSecretAsync(secretName);
            // You only need to wait for completion if you want to purge or recover the secret.
            await operation.WaitForCompletionAsync();
            Console.WriteLine(" done.");

            Console.Write($"Purging your secret from {keyVaultName} ...");
            await client.PurgeDeletedSecretAsync(secretName);
            Console.WriteLine(" done.");
        }
    }
}
