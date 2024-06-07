using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ITI.gRPC.Client.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ITI.gRPC.Client.Protos.ProductService;

namespace ITI.gRPCDay1Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7133");
            var client = new ProductServiceClient(channel);
            var IsExisted = await client.GetProductAsync(new ProductId { Value = product.Id });
            if (IsExisted.Exist == false)
            {
                await client.AddProductAsync(product);
                return Created();
            }
            await client.UpdateProductAsync(product);
            return Ok(product);

        }

        [HttpPost]
        [Route("AddBulkProducts")]
        public async Task<ActionResult> AddBulkProducts(List<Product> products)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7133");
            var client = new ProductServiceClient(channel);
            var call = client.AddBulkProducts();
            try
            {
                foreach (var item in products)
                {
                    await call.RequestStream.WriteAsync(new Product
                    {
                        Id = item.Id,
                        Price = item.Price,
                        Name = item.Name,
                        Quantity = item.Quantity

                    });
                }
                await call.RequestStream.CompleteAsync();
                return Ok(call.ResponseAsync.Result.Value);//return num of inserted products
            }
           
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult> GenerateProductReport([FromQuery] FilterDetails filter)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7133");
            var client = new ProductServiceClient(channel);
            var call = client.GenerateProductReport(filter);
            List<Product> products = new List<Product>();

            await foreach (var item in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(item.Name);
                products.Add(item);
            }
            return Ok(products);

        }
        }
    }
