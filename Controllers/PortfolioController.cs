using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Command;
using Portfolio.Command.Handler;

namespace Investments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfolioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> OperatePortfolio([FromBody] OperateProductCustomerCommand command, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ulong? customerId, CancellationToken cancellationToken = default)
        {
            if(customerId == null)
            {
                var response = await _mediator.Send(new GetPortfolioAllCustomersQuery(), cancellationToken);
                return Ok(response);

            } else
            {
                var response = await _mediator.Send(new GetPortfolioByCustomerQuery(customerId??0), cancellationToken);
                return Ok(response);
            }

        }
    }
}
