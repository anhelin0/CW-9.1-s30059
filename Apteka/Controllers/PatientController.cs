using Microsoft.AspNetCore.Mvc;
using Apteka.Exceptions;
using Apteka.Services;

namespace Pharmacy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController(IDbService dbService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient([FromRoute] int id)
    {
        try
        {
            var result = await dbService.GetPatientAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}