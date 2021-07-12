using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ToDOFunctionApp.Services;
using ToDOFunctionApp.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace ToDOFunctionApp
{
    public class Function1
    {
        private readonly CosmosDbService cdb;
        public Function1(CosmosDbService cdbs)
        {
            this.cdb = cdbs;
        }
        //[FunctionName("GetToDoItem")]
        //public async Task<IActionResult> GetToDoItem(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["TaskId"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    name = name == null ? "5c86dc01-a2bc-4f2a-afd1-c1191ebba1dd" : "";


        //    //string responseMessage = string.IsNullOrEmpty(name)
        //    //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //    //    : $"Hello, {name}. This HTTP triggered function executed successfully.";
        //    var responseMessage = await cdb.GetItemAsync(name);

        //    return new OkObjectResult(responseMessage);
        //}


        [FunctionName("Create_ToDo")]
        public async Task CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] Item todoItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            //var todoItem =  req.Content.ReadAsAsync<Item>().Result;
            //req.Content.
            //var todoItem = JsonConvert.DeserializeObject<Item>(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";
            todoItem.id = Guid.NewGuid().ToString();
            await cdb.AddItemAsync(todoItem);
            //return new OkObjectResult(responseMessage);
        }


        [FunctionName("GetAll")]
        public async Task<IEnumerable<Item>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            return await cdb.GetItemsAsync("SELECT * FROM c");
            //return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetTaskItem")]
        public async Task<ActionResult<Item>> GetTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTaskItem/{id}")] HttpRequestMessage req,string id,
            ILogger log)
        {
            if (id == null || String.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            Item item = await cdb.GetItemAsync(id);
            if(item == null)
            {
                return new BadRequestResult();
            }
            return item;
            //return new OkObjectResult(responseMessage);
        }

        [FunctionName("EditTaskItem")]
        public async Task<ActionResult> EditTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] Item taskitem,
            ILogger log)
        {
            if (taskitem.id == null || String.IsNullOrEmpty(taskitem.id))
            {
                return new BadRequestResult();
            }

            await cdb.UpdateItemAsync(taskitem.id, taskitem);
            //return OkResult;
            return new ObjectResult($"Task Item {taskitem.id} modified !");
        }

        [FunctionName("DeleteTaskItem")]
        public async Task<ActionResult<Item>> DeleteTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "GetTaskItem/{id}")] HttpRequestMessage req, string id,
            ILogger log)
        {
            if (id == null || String.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            await cdb.DeleteItemAsync(id);
            return new OkObjectResult($"Task Item {id} deleted !");
            //return new OkObjectResult(responseMessage);
        }

    }
}
