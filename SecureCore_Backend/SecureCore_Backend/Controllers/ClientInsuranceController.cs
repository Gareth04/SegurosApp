using Microsoft.AspNetCore.Mvc;
using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using SecureCore_Backend.Services;

namespace SecureCore_Backend.Controllers
{
    [ApiController]
    [Route("api/ClientInsurance")]
    public class ClientInsuranceController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ClientInsuranceServices clientInsuranceServices;
        private Response response;
        public ClientInsuranceController(ApplicationDbContext context, ClientInsuranceServices clientInsuranceServices)
        {
            this.context = context;
            this.clientInsuranceServices = clientInsuranceServices;
            response = new Response();
        }
        [HttpGet]
        public async Task<Response> GetClients()
        {
            return await clientInsuranceServices.GetClients();
        }
        [HttpPost("addInsurance")]
        public async Task<Response> CreateClientInsurance(string cedula, string insuranceName)
        {
            return await clientInsuranceServices.AddInsuranceToClientAsync(cedula, insuranceName);
        }
        [HttpPost("api/file/clientInsurance")]
        public async Task<Response> UploadFileClientInsurance(IFormFile file)
        {
            return await clientInsuranceServices.UploadFileClientInsurance(file);
        }

        [HttpGet("GetClientInsuranceDetails")]
        public async Task<Response> GetClientInsuranceDetails(string cedula)
        {
            return await clientInsuranceServices.GetClientInsuranceDetailsAsync(cedula);
        }
        [HttpGet("GetClientDetailsByInsuranceCode")]
        public async Task<Response> GetClientDetailsByInsuranceCode(int insuranceCode)
        {
            return await clientInsuranceServices.GetClientsByInsuranceCodeAsync(insuranceCode);
        }
        [HttpDelete("removeInsurance")]
        public async Task<Response> RemoveInsuranceFromClient(string cedula, string insuranceName)
        {
            return await clientInsuranceServices.RemoveInsuranceFromClientAsync(cedula, insuranceName);
        }

    }
}
