using MediatR;
using Microsoft.AspNetCore.Mvc;
using Products.Command;
using Products.Query;
using Products.Repository.Interface;

namespace Investments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IReadProductRepository _productReadRepository;

        public ProductController(IMediator mediator, IReadProductRepository productReadRepository)
        {
            _mediator = mediator;
            _productReadRepository = productReadRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken = default)
        //public async Task<IActionResult> InsertProduct([FromQuery] Guid id, [FromBody] string name, string productType, ulong unitPrice, ulong availableQuantity, CancellationToken cancellationToken = default)
        {
            //if (id == Guid.Empty) return NoContent();
            //command.Id = id;
            //UpdateProductCommand command = new(id, name, productType, unitPrice, availableQuantity);

            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? productName, CancellationToken cancellationToken = default)
        {
            if(productName == null)
            {
                var response = await _mediator.Send(new GetAllProductQuery(), cancellationToken);
                return Ok(response);

            } else
            {
                var response = await _mediator.Send(new GetProductByNameQuery(productName), cancellationToken);
                return Ok(response);
            }

        }
        [HttpGet("statement")]
        public async Task<IActionResult> GetStatementByName([FromQuery] string? user, [FromQuery] string? productName, [FromQuery] DateTime? expirationDate, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetStatementByProductQuery(user, productName, expirationDate), cancellationToken);

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            DeleteProductCommand command = new DeleteProductCommand();
            command.Id = id;
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
