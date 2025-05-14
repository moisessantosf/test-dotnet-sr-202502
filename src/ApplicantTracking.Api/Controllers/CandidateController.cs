using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate;
using ApplicantTracking.Application.Features.Candidates.Commands.DeleteCandidate;
using ApplicantTracking.Application.Features.Candidates.Commands.UpdateCandidate;
using ApplicantTracking.Application.Features.Candidates.Queries.GetAllCandidates;
using ApplicantTracking.Application.Features.Candidates.Queries.GetCandidateById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<CandidateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var query = new GetAllCandidatesQuery();
            var candidates = await _mediator.Send(query);
            return Ok(candidates);
        }

        [HttpGet("{idCandidate:int}")]
        [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int idCandidate)
        {
            var query = new GetCandidateByIdQuery(idCandidate);
            var candidate = await _mediator.Send(query);
            if (candidate == null)
            {
                return NotFound(new { message = $"Candidate with id {idCandidate} not found." });
            }
            return Ok(candidate);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateCandidateDto createCandidateDto)
        {
            var command = new CreateCandidateCommand(createCandidateDto);
            var candidateDto = await _mediator.Send(command);

            return CreatedAtAction(nameof(Get), new { idCandidate = candidateDto.IdCandidate }, candidateDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateCandidateDto)
        {
            var command = new UpdateCandidateCommand(id, updateCandidateDto);
            var result = await _mediator.Send(command);
            if (result == null)
            {
                return NotFound(new { message = $"Candidate with id {id} not found for update." });
            }
            return NoContent();
        }

        [HttpDelete("{idCandidate:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int idCandidate)
        {
            var command = new DeleteCandidateCommand(idCandidate);
            var success = await _mediator.Send(command);
            if (!success)
            {
                return NotFound(new { message = $"Candidate with id {idCandidate} not found for deletion." });
            }
            return NoContent();
        }
    }
}
