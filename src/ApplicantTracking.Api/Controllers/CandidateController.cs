using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using ApplicantTracking.Application.Commands;
using ApplicantTracking.Application.Queries;

namespace ApplicantTracking.Api.Controllers
{
    [ApiController]
    [Route("candidates")]
    public sealed class CandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var query = new GetCandidatesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{idCandidate:int}")]
        public async Task<IActionResult> Get([FromRoute] int idCandidate)
        {
            var query = new GetCandidateByIdQuery { Id = idCandidate };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCandidateCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { idCandidate = result }, null);
        }

        [HttpPut("{idCandidate:int}")]
        public async Task<IActionResult> Edit([FromRoute] int idCandidate, [FromBody] UpdateCandidateCommand command)
        {
            if (idCandidate != command.Id)
                return BadRequest();

            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{idCandidate:int}")]
        public async Task<IActionResult> Delete([FromRoute] int idCandidate)
        {
            var command = new DeleteCandidateCommand { Id = idCandidate };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}