using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using CommandLine;
using CsvHelper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableStorageBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CopyOptions>(args).WithParsed(ExportTable);
        }

        static void ExportTable(CopyOptions options)
        {
            var creds = new StorageCredentials(options.StorageAccountName, options.StorageAccountKey);
            var account = new CloudStorageAccount(creds, true);

            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference(options.TableName);

            TableQuerySegment segment = null;
            var allResults = new List<DynamicTableEntity>();
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = table.ExecuteQuerySegmentedAsync(new TableQuery(), segment?.ContinuationToken).Result;
                allResults.AddRange(segment.Results);
            }

            var writer = new StreamWriter($"{Environment.CurrentDirectory}\\{options.CsvTableName}");
            var csv = new CsvWriter(writer);
            foreach (var result in allResults)
            {
                var record = new ExpandoObject() as IDictionary<string, object>;
                record.Add("PartitionKey", result.PartitionKey);
                record.Add("RowKey", result.PartitionKey);
                foreach (var property in result.Properties.OrderBy(p => p.Key))
                {
                    record.Add(property.Key, property.Value.StringValue);
                }

                csv.WriteRecord(record);
                csv.NextRecord();
            }
            csv.Flush();
            writer.Flush();
            csv.Dispose();
            writer.Dispose();
        }
    }
}