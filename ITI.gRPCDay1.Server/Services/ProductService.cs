using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.gRPCDay1.Server.Protos;
using static ITI.gRPCDay1.Server.Protos.ProductService;
namespace ITI.gRPCDay1.Server.Services

{
    public class ProductService:ProductServiceBase
    {
         static List<Product> Products = new List<Product>();
        public override async Task<ResponseState> GetProduct(ProductId request, ServerCallContext context)
        {
            bool exist = Products.Any(p => p.Id == request.Value);
            
            return  await Task.FromResult(new ResponseState { Exist=exist});
        }
        public override async Task<Product> AddProduct(Product request, ServerCallContext context)
        {
            Products.Add(request);
            return await Task.FromResult(new Product { Id=request.Id,Name=request.Name,Quantity=request.Quantity,Price=request.Price});
        }
        public override async Task<Product> UpdateProduct(Product request, ServerCallContext context)
        {
            var product=Products.FirstOrDefault(p=>p.Id==request.Id);
            
                product.Name = request.Name;
                product.Price = request.Price;
                product.Quantity = request.Quantity;

            
            return await Task.FromResult(new Product { Id = request.Id, Name = request.Name, Quantity = request.Quantity, Price = request.Price });
        }

        public override async Task<NumOfProducts> AddBulkProducts(IAsyncStreamReader<Product> requestStream, ServerCallContext context)
        {
            int NumOfInsertedProduct = 0;
            await foreach (var request in requestStream.ReadAllAsync())
            {
                Products.Add(request);
                NumOfInsertedProduct++;
            }

            return (new NumOfProducts { Value=NumOfInsertedProduct });
        }
        public override async Task GenerateProductReport(FilterDetails request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            List<Product> products;
            products = Products.Where(x => x.Category == request.IsGroupedBySpecificCatergory).ToList();
            if (request.OrderByPrice)
            {
                Products = Products.OrderBy(p => p.Price).ToList();
            }
            foreach (var item in Products)
            {
                await responseStream.WriteAsync(item);
            }
        }

    }
   
}
