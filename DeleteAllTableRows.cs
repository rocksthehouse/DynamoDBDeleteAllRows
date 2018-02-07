using System.Collections.Generic;
using System.Linq;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDeleteAllRows
{
    /// <summary>
    /// Class for deleting all rows from an AWS DynamoDB table.
    /// </summary>
    internal class DeleteAllTableRows
    {
        // This is fixed by AWS.
        private const int maxBatchSize = 25;

        private readonly List<WriteRequest> deleteRequestList = new List<WriteRequest>();

        public void DeleteAllRows(string tableName, string hashKey, string rangeKey)
        {
            bool rangeKeyExists = !string.IsNullOrWhiteSpace(rangeKey);

            using (var dbClient = new AmazonDynamoDBClient())
            {
                var scanRequest = new ScanRequest(tableName) { ProjectionExpression = rangeKeyExists ? $"{hashKey},{rangeKey}" : $"{hashKey}" };

                ScanResponse scanResponse;     

                while ((scanResponse = dbClient.Scan(scanRequest)).ScannedCount > 0)
                {
                    foreach (var item in scanResponse.Items)
                    {
                        var deleteKeys = new Dictionary<string, AttributeValue> { { hashKey, item[hashKey] } };

                        if (rangeKeyExists)
                        {
                            deleteKeys.Add(rangeKey, item[rangeKey]);
                        }

                        if (deleteRequestList.Count >= maxBatchSize)
                        {
                            BatchDelete(dbClient, tableName, deleteRequestList);
                        }

                        deleteRequestList.Add(new WriteRequest(new DeleteRequest(deleteKeys)));
                    }

                    BatchDelete(dbClient, tableName, deleteRequestList);

                    scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
                }
            }
        }

        private static void BatchDelete(IAmazonDynamoDB dbClient, string tableName, List<WriteRequest> deleteRequestList)
        {
            if (deleteRequestList.Any())
            {
                var itemsToDelete = new Dictionary<string, List<WriteRequest>> { { tableName, deleteRequestList } };

                var batchDeleteRequest = new BatchWriteItemRequest
                {
                    RequestItems = itemsToDelete
                };

                dbClient.BatchWriteItem(batchDeleteRequest);
            }

            deleteRequestList.Clear();
        }
    }
}
