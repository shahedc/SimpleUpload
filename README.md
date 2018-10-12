# SimpleUpload
ASP .NET Core web app upload from browser to Azure Storage Account

**Instructions**
1. Rename placeholder config file "appsettings.Development.txt" to "appsettings.Development.json"
2. Replace placeholder string "<REPLACE_CONN_STRING>" with your Azure Storage Account connection string.

**References**
1. ASP .NET Core File Upload with Form POST: https://www.youtube.com/watch?v=dZFucw0Vq9w
2. Convert file to byte array: https://stackoverflow.com/questions/36432028/how-to-convert-a-file-into-byte-array-directly-without-its-pathwithout-saving-f
3. How to access config from Controller: https://blogs.technet.microsoft.com/dariuszporowski/tip-of-the-week-how-to-access-configuration-from-controller-in-asp-net-core-2-0/
4. Docs article on using .NET to create a blob in object storage: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows
5. Github rep for blob access with .NET : https://github.com/Azure-Samples/storage-blobs-dotnet-quickstart
6. Using Azure Storage in ASP .NET Core: https://wildermuth.com/2017/10/19/Using-Azure-Storage-in-ASP-NET-Core
7. Docs on UploadFromByteArrayAsync: https://docs.microsoft.com/en-us/dotnet/api/microsoft.windowsazure.storage.blob.cloudblockblob.uploadfrombytearrayasync?view=azure-dotnet
