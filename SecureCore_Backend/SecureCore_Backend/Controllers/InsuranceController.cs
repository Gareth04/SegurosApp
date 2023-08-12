using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using SecureCore_Backend.Services;
using System.ComponentModel;


namespace SecureCore_Backend.Controllers
{
    [ApiController]
    [Route("api/Insurance")]
    public class InsuranceController : ControllerBase
    {
        private readonly InsuranceServices insuranceService;
        private readonly ApplicationDbContext context;
        private Response response;
        public InsuranceController(ApplicationDbContext context, InsuranceServices insuranceService)
        {
            this.insuranceService = insuranceService;
            this.context = context;
            response = new Response();

        }

        //Get 
        [HttpGet]
        public async Task<Response> GetListInsurance()
        {
            return await insuranceService.GetListInsuranceAsync();
        }

        [HttpGet("nombre")]
        public async Task<Response> GetByName(string name)
        {
            return await insuranceService.GetInsuranceByName(name);
        }

        [HttpGet("id")]
        public async Task<Response> GetById(int id)
        {
            return await insuranceService.GetInsuranceById(id);
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> CreateInsurance([FromBody] Insurance insuranceData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await insuranceService.CreateInsuranceAsync(insuranceData));
        }

        [HttpPost("api/file")]
        public async Task<Response> UploadFile(IFormFile file)
        {
            return await insuranceService.UploadFileInsurance(file);
        }

        //Put
        [HttpPut("{id}")]
        public async Task<Response> UpdateInsurance(int id, [FromBody] Insurance insurance)
        {
            if (!ModelState.IsValid)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Ingrese los datos correctamente"
                };
            }

            return await insuranceService.UpdateInsuranceAsync(id, insurance);
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<Response> DeleteInsurance(int id)
        {
            return await insuranceService.DeleteInsuranceAsync(id);
        }
    }
}
