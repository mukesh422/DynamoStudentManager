using Amazon.DynamoDBv2.DataModel;
using Amazon.S3.Model;
using Amazon.S3;
using DynamoStudentManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using DynamoStudentManager.ViewModel;

namespace DynamoStudentManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UtilitiesController : ControllerBase
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonS3 _amazonS3;

    public UtilitiesController(IDynamoDBContext context, IAmazonS3 amazonS3)
    {
        _context = context;
        _amazonS3 = amazonS3;
    }

    [HttpGet("api/countries/{countryId}")]
    public async Task<IActionResult> GetById(int countryId)
    {
        var country = await _context.LoadAsync<Country>(countryId);
        if (country == null) return NotFound();
        return Ok(country);
    }

    [HttpGet("api/countries")]
    public async Task<IActionResult> GetAllCountries()
    {
        var country = await _context.ScanAsync<Country>(default).GetRemainingAsync();
        return Ok(country);
    }

    [HttpPost("api/countries")]
    public async Task<IActionResult> CreateCountry(Country countryRequest)
    {
        var country = await _context.LoadAsync<Country>(countryRequest.Id);
        if (country != null) return BadRequest($"Country with Id {countryRequest.Id} Already Exists");
        await _context.SaveAsync(countryRequest);
        return Ok(countryRequest);
    }

    [HttpDelete("api/countries/{countryId}")]
    public async Task<IActionResult> DeleteCountry(int countryId)
    {
        var country = await _context.LoadAsync<Country>(countryId);
        if (country == null) return NotFound();
        await _context.DeleteAsync(country);
        return NoContent();
    }

    [HttpPut("api/countries/{countryId}")]
    public async Task<IActionResult> UpdateCountry(Country countryRequest)
    {
        var student = await _context.LoadAsync<Country>(countryRequest.Id);
        if (student == null) return NotFound();
        await _context.SaveAsync(countryRequest);
        return Ok(countryRequest);
    }

    [HttpGet("{stateId}")]
    public async Task<IActionResult> GetStateById(int stateId)
    {
        var state = await _context.LoadAsync<Country>(stateId);
        if (state == null) return NotFound();
        return Ok(state);
    }

    [HttpGet("state")]
    public async Task<IActionResult> GetAllStates()
    {
        var state = await _context.ScanAsync<State>(default).GetRemainingAsync();
        return Ok(state);
    }

    [HttpPost("{state}")]
    public async Task<IActionResult> CreateState(State stateRequest)
    {
        var state = await _context.LoadAsync<State>(stateRequest.Id);
        if (state != null) return BadRequest($"State with Id {stateRequest.Id} Already Exists");
        await _context.SaveAsync(stateRequest);
        return Ok(stateRequest);
    }

    [HttpDelete("{stateId}")]
    public async Task<IActionResult> DeleteState(int stateId)
    {
        var state = await _context.LoadAsync<State>(stateId);
        if (state == null) return NotFound();
        await _context.DeleteAsync(state);
        return NoContent();
    }

    [HttpPut("state")]
    public async Task<IActionResult> UpdateState(State stateRequest)
    {
        var state = await _context.LoadAsync<State>(stateRequest.Id);
        if (state == null) return NotFound();
        await _context.SaveAsync(stateRequest);
        return Ok(stateRequest);
    }




    [HttpGet("api/cities/{cityId}")]
    public async Task<IActionResult> GetByCityId(int cityId)
    {
        var city = await _context.LoadAsync<City>(cityId);
        if (city == null) return NotFound();
        return Ok(city);
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetAllCities()
    {
        var city = await _context.ScanAsync<City>(default).GetRemainingAsync();
        return Ok(city);
    }

    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity(City cityRequest)
    {
        var city = await _context.LoadAsync<City>(cityRequest.Id);
        if (city != null) return BadRequest($"City with Id {cityRequest.Id} Already Exists");
        await _context.SaveAsync(cityRequest);
        return Ok(cityRequest);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCity(int cityId)
    {
        var city = await _context.LoadAsync<City>(cityId);
        if (city == null) return NotFound();
        await _context.DeleteAsync(city);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCity(City cityRequest)
    {
        var city = await _context.LoadAsync<City>(cityRequest.Id);
        if (city == null) return NotFound();
        await _context.SaveAsync(cityRequest);
        return Ok(cityRequest);
    }





    [HttpGet("api/employees/{employeeId}")]
    public async Task<IActionResult> GetByEmployeeId(int employeeId)
    {
        var emp = await _context.LoadAsync<Employee>(employeeId);
        if (emp == null) return NotFound();
        return Ok(emp);
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var emp = await _context.ScanAsync<Employee>(default).GetRemainingAsync();
        return Ok(emp);
    }


    [HttpPost("employeenew")]
    public async Task<IActionResult> CreateEmployeeNew(Employee employeeRequest)
    {
        await _context.SaveAsync(employeeRequest);
        return Ok(employeeRequest);
    }


    [HttpPost("employee")]
    public async Task<IActionResult> CreateEmployee(EmployeeVM employeevm)
    {
        try
        {
            Employee employeeRequest = new Employee()
            {
                Id = employeevm.Id,
                Name = employeevm.Name,
                Email = employeevm.Email,
                Phone = employeevm.Phone,
                City = employeevm.City,
                Technology_ID = employeevm.Technology_ID,
                Description = employeevm.Description
            };

            var emp = await _context.LoadAsync<Employee>(employeeRequest.Id);
            if (emp != null)
                return BadRequest($"Employee with Id {employeeRequest.Id} Already Exists");

            var prefix = string.Empty;

            if (employeeRequest.Technology_ID == 1)
                prefix = "dotnet";

            if (employeeRequest.Technology_ID == 2)
                prefix = "java";

            if (employeeRequest.Technology_ID == 3)
                prefix = "react";

            if (employeeRequest.Technology_ID == 4)
                prefix = "angular";

            if (employeeRequest.Technology_ID == 5)
                prefix = "aws";

            if (employeeRequest.Technology_ID == 6)
                prefix = "azure";

            var s3Request = new PutObjectRequest
            {
                BucketName = "shiwanshresumebucket",
                Key = prefix + "/" + employeevm.File.FileName,
                InputStream = employeevm.File.OpenReadStream()
            };

            // Set content type based on file's ContentType property
            s3Request.ContentType = employeevm.File.ContentType;

            await _amazonS3.PutObjectAsync(s3Request);

            // Store employee data in DynamoDB
            await _context.SaveAsync(employeeRequest);

            return Ok($"You are registered successfully!!");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception: {ex}");

            // Return an error response
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [HttpDelete("employee")]
    public async Task<IActionResult> DeleteEmployee(int employeeId)
    {
        var emp = await _context.LoadAsync<Employee>(employeeId);
        if (emp == null) return NotFound();
        await _context.DeleteAsync(emp);
        return NoContent();
    }

    [HttpPut("employee")]
    public async Task<IActionResult> UpdateEmployee(Employee employeeRequest)
    {
        var emp = await _context.LoadAsync<Employee>(employeeRequest.Id);
        if (emp == null) return NotFound();
        await _context.SaveAsync(employeeRequest);
        return Ok(employeeRequest);
    }
}
