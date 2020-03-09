/*
This code is for
1. Azure function app
2. generating the SAS token for a container
3. storing the SAS token in Key vault.
*/


using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=dontknow;AccountKey=LniZz9980s2RaURg3UYak+jhuz2M347eacog61NphdNKn17I9SBIq72P/oeMFtQrefxKCWIxIo2j+DvIVeZXHLi+g==;EndpointSuffix=core.windows.net");

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("container01");


            SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
            };

            string sasContainerToken = cloudBlobContainer.GetSharedAccessSignature(adHocPolicy, null);

            ////
            ///

            var keyVaultEndpoint = GetKeyVaultEndpoint();
            var client = new SecretClient(new Uri(keyVaultEndpoint), new DefaultAzureCredential());

            client.SetSecret("testcontainerkey", sasContainerToken);

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private static string GetKeyVaultEndpoint() => "https://testfunckeyvault.vault.azure.net/";
    }
}
