using Microsoft.AspNetCore.Mvc;
using Apteka.DTOs;
using Apteka.Exceptions;
using Apteka.Services;

namespace Pharmacy.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PrescriptionController(IDbService dbService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] NewPrescriptionDTO dto)
    {
        try
        {
            await dbService.AddPrescriptionAsync(dto);
            return Created("", null);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}