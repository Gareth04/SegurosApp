using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using SecureCore_Backend.Services;

namespace SecureCore_Backend.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ClientServices clientServices;
        private Response response;
        public ClientController(ApplicationDbContext context, ClientServices clientServices)
        {
            this.clientServices = clientServices;
            this.context = context;
            response = new Response();
        }

        //Get
        [HttpGet]
        public async Task<Response> ListClient()
        {
            return await clientServices.GetClientList();
        }


        //Post
        [HttpPost]
        public async Task<Response> CreateClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Ingrese los datos correctamente"
                };
            }
            return await clientServices.CreateClient(client);
        }
        [HttpPost("api/file")]
        public async Task<Response> UploadFile(IFormFile file)
        {
            return await clientServices.UploadFileClient(file);
        }

        //Put
        [HttpPut("{id}")]
        public async Task<Response> UpdateClient(int id, [FromBody] Client clientUpdate)
        {
            if (!ModelState.IsValid)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Ingrese los datos correctamente"
                };
            }

            return await clientServices.UpdateClient(id, clientUpdate);
        }

        //Delete
        [HttpDelete]
        public async Task<Response> DeleteClient(int id)
        {
            return await clientServices.DeleteClient(id);
        }
    }
}
