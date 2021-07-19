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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ToDOFunctionApp
{
    class Response
    {
        public string Message { get; set; }
    }
    public class Function1
    {
        private readonly CosmosDbService cdb;
        public Function1(CosmosDbService cdbs)
        {
            this.cdb = cdbs;
        }

        [FunctionName("Create_ToDo")]
        [OpenApiOperation(operationId: "Create ToDo Item", tags: new[] { "Create ToDo Item" })]
        [OpenApiParameter(name: "item", Required = true, Type = typeof(Item), Description = "Example Item object: {\"name\":\"create ADO task for review\",\"description\":\"security review for PJA\",\"isComplete\":false}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Response), Summary = "Returns ID of created ToDo item")]
        public async Task<IActionResult> CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] Item todoItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            todoItem.id = Guid.NewGuid().ToString();
            await cdb.AddItemAsync(todoItem);
            var r = new Response() { Message = $"Task Item {todoItem.id} created !" };
            return new OkObjectResult(r);
        }


        [FunctionName("GetAll")]
        [OpenApiOperation(operationId: "Get All Items", tags: new[] { "Get All Items" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Item>), Summary = "Returns ID of created ToDo item")]
        public async Task<IEnumerable<Item>> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            return await cdb.GetItemsAsync("SELECT * FROM c");
            //return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetTaskItem")]
        [OpenApiOperation(operationId: "Get a Task Item", tags: new[] { "Get Task Item" })]
        [OpenApiParameter(name: "id", Required = true, In = ParameterLocation.Path, Type = typeof(string), Description = "Example Item ID: a94c0dff-e50b-4309-9e87-f222f7dcbe00")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Summary = "Returns a ToDo item")]
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
        [OpenApiOperation(operationId: "Edit Task Item", tags: new[] { "Edit Task Item" })]
        [OpenApiParameter(name: "Item", Required = true, Type = typeof(Item), Description = "Example Item object: {\"id\":\"0c17382c-7325-4b4d-bdb3-14d5f82f9954\",\"name\":\"create ADO task for review\",\"description\":\"security review for PJA\",\"isComplete\":false}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Response), Summary = "Returns ID of created ToDo item")]
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
            var r = new Response() { Message = $"Task Item {taskitem.id} modified !" };
            return new ObjectResult(r);
        }

        [FunctionName("DeleteTaskItem")]
        [OpenApiOperation(operationId: "Delete Task Item", tags: new[] { "DeleteTaskItem" })]
        [OpenApiParameter(name: "id", Required = true, In = ParameterLocation.Path, Type = typeof(string), Description = "Example Item ID: a94c0dff-e50b-4309-9e87-f222f7dcbe00")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Summary = "Deletes a ToDo item")]
        public async Task<ActionResult<Item>> DeleteTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "GetTaskItem/{id}")] HttpRequestMessage req, string id,
            ILogger log)
        {
            if (id == null || String.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            await cdb.DeleteItemAsync(id);
            var r = new Response() { Message = $"Task Item {id} deleted !" };
            return new OkObjectResult(r);
            //return new OkObjectResult(responseMessage);
        }

    }
}
