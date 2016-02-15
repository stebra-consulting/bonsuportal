using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using BonusPortal.Models;

namespace BonsuPortal
{
    public class AzureManager
    {
        //Config
        public const string tableName = "myNewTable";
        public const string partitionKey = "consultant";

        //Connection to Azure Storage
        private static CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //get Table -client  
        public static CloudTableClient tableClient = StorageAccount.CreateCloudTableClient();

        public static CloudTable table = tableClient.GetTableReference(tableName);

        public static IEnumerable<Consultant> LoadData()
        {

            //Query all entities where "PartitionKey" is "consultant"
            var allNewsQuery = new TableQuery<Consultant>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var consultants = table.ExecuteQuery(allNewsQuery).ToList();

            List<string> dates = new List<string>();

            for (int i = 0; i < consultants.Count(); i++)
            {
                consultants[i].LoyaltyFactor = consultants[i].loyalty().ToString();
            }

            return consultants;
        }

        public static void AddData(Consultant item)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(item);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public static void DeleteData(Consultant item)
        {

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<Consultant>(partitionKey, item.RowKey);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            Consultant deleteThisItem = (Consultant)retrievedResult.Result;

            if (deleteThisItem != null)
            {
                // Delete
                TableOperation deleteOperation = TableOperation.Delete(deleteThisItem);

                // Execute the operation.
                table.Execute(deleteOperation);

                Console.WriteLine("Entity deleted.");
            }

            else { Console.WriteLine("Entity could not be deleted."); }

        }

        public static void EditData(Consultant item)
        {

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<Consultant>(partitionKey, item.RowKey);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            Consultant replaceThisItem = (Consultant)retrievedResult.Result;

            if (replaceThisItem != null)
            {
                // Change the name.
                replaceThisItem.Name = item.Name;
                replaceThisItem.EmploymentDate = item.EmploymentDate;

                // Create the InsertOrReplace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(replaceThisItem);

                // Execute the operation.
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }

            else { Console.WriteLine("Entity could not be retrieved."); }
               
        }

    }

}