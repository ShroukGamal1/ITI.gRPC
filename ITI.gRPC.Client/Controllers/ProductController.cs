using Grpc.Net.Client;
using ITI.gRPC.Client.Models;
using ITI.gRPC.Client.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ITI.gRPC.Client.Protos.ProductService;

namespace ITI.gRPC.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult>AddProduct(Product product) {
            var channel = GrpcChannel.ForAddress("https://localhost:5043");
            var client =new ProductServiceClient(channel);
            var IsExisted= await client.GetProductAsync(new ProductId { Value = product.Id });
            if(IsExisted.Exist == false)
            {
                await client.AddProductAsync(product);
                return CreatedAtAction("GetProduct",new {ProductId=product.Id},product);
            }
            await client.UpdateProductAsync(product);
            return Ok(product);
        
        }
    }
}
