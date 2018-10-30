using CommandLine;

namespace AzureTableStorageBackup
{
    internal class CopyOptions
    {
        [Option(
            "sn",
            Required = true,
            HelpText = "Azure storage account name")]
        public string StorageAccountName { get; set; }

        [Option(
            "sk",
            Required = true,
            HelpText = "Azure storage account key")]
        public string StorageAccountKey { get; set; }

        [Option(
            "tn",
            Required = true,
            HelpText = "Azure storage table name")]
        public string TableName { get; set; }

        [Option(
            "csv",
            Required = true,
            HelpText = "Exported csv name")]
        public string CsvTableName { get; set; }
    }
}