using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using OfficeOpenXml;

namespace SecureCore_Backend.Services
{
    public class ClientInsuranceServices
    {
        private readonly ApplicationDbContext dbContext;
        public ClientInsuranceServices(ApplicationDbContext context) 
        {
            this.dbContext = context;
        }
        public async Task<Response> GetClients()
        {
            try
            {
                var clientInsurance = await dbContext.ClientInsurance
                    .Include(ci => ci.Client)
                    .Include(ci => ci.Insurance)
                    .ToListAsync();

                if (clientInsurance.IsNullOrEmpty())
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "No existen registros de cliente-seguro",
                    };
                }

                var simplifiedData = clientInsurance.Select(ci => new
                {
                    ClientName = ci.Client.name,
                    InsuranceName = ci.Insurance.name,
                    SumInsured = ci.Insurance.sum_Insured,
                    Premium = ci.Insurance.Premium
                }).ToList();

                return new Response()
                {
                    Code = "00",
                    Message = "Lista de registros de cliente-seguro generada con éxito",
                    Data = simplifiedData
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = ex.Message
                };
            }
        }



        public async Task<Response> AddInsuranceToClientAsync(string cedula, string insuranceName)
        {
            try
            {
                var client = await dbContext.Client.FirstOrDefaultAsync(c => c.cedula == cedula);

                if (client != null)
                {
                    var insurance = await dbContext.Insurance.FirstOrDefaultAsync(i => i.name == insuranceName);

                    if (insurance != null)
                    {
                        var existingClientInsurance = await dbContext.ClientInsurance
                            .FirstOrDefaultAsync(ci => ci.Id_Client == client.id_client && ci.Id_Insurance == insurance.id);

                        if (existingClientInsurance != null)
                        {
                            return new Response
                            {
                                Code = "03",
                                Message = "El cliente ya tiene este seguro asociado"
                            };
                        }

                        var newClientInsurance = new ClientInsurance
                        {
                            Id_Client = client.id_client,
                            Id_Insurance = insurance.id
                        };

                        dbContext.ClientInsurance.Add(newClientInsurance);
                        await dbContext.SaveChangesAsync();

                        return new Response
                        {
                            Code = "00",
                            Message = "Seguro asociado al cliente con éxito"
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            Code = "02",
                            Message = "Seguro no encontrado"
                        };
                    }
                }
                else
                {
                    return new Response
                    {
                        Code = "02",
                        Message = "Cliente no encontrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "02",
                    Message = "Error al asociar el seguro al cliente"
                };
            }
        }
        public async Task<Response> UploadFileClientInsurance(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "No se proporcionó ningún archivo"
                    };
                }

                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet != null)
                    {
                        int rowCount = worksheet.Dimension?.Rows ?? 0;

                        if (rowCount <= 1)
                        {
                            return new Response()
                            {
                                Code = "02",
                                Message = "El archivo no contiene datos"
                            };
                        }

                        try
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string seguroName = worksheet.Cells[row, 2]?.Value?.ToString();
                                string cedula = worksheet.Cells[row, 3]?.Value?.ToString();

                                if (string.IsNullOrEmpty(seguroName) || string.IsNullOrEmpty(cedula))
                                {
                                    return new Response()
                                    {
                                        Code = "02",
                                        Message = "Valores de nombre de seguro y/o cédula inválidos en la fila " + row
                                    };
                                }

                                var client = await dbContext.Client.FirstOrDefaultAsync(c => c.cedula == cedula);
                                var insurance = await dbContext.Insurance.FirstOrDefaultAsync(i => i.name == seguroName);

                                if (client != null && insurance != null)
                                {
                                    var existingClientInsurance = await dbContext.ClientInsurance
                                        .FirstOrDefaultAsync(ci => ci.Id_Client == client.id_client && ci.Id_Insurance == insurance.id);

                                    if (existingClientInsurance != null)
                                    {
                                        return new Response
                                        {
                                            Code = "03",
                                            Message = "El cliente ya tiene este seguro asociado en la fila " + row
                                        };
                                    }

                                    var newClientInsurance = new ClientInsurance
                                    {
                                        Id_Client = client.id_client,
                                        Id_Insurance = insurance.id
                                    };

                                    dbContext.ClientInsurance.Add(newClientInsurance);
                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    return new Response()
                                    {
                                        Code = "02",
                                        Message = "Cliente o seguro no encontrado en la fila " + row
                                    };
                                }
                            }

                            return new Response()
                            {
                                Code = "00",
                                Message = "Archivo subido correctamente",
                            };
                        }
                        catch (Exception ex)
                        {
                            return new Response()
                            {
                                Code = "02",
                                Message = "Error al registrar archivo"
                            };
                        }
                    }
                    else
                    {
                        return new Response()
                        {
                            Code = "02",
                            Message = "La hoja de cálculo no se encontró en el archivo"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al procesar el archivo"
                };
            }
        }



        public async Task<Response> GetClientInsuranceDetailsAsync(string cedula)
        {
            try
            {
                var insuranceDetails = await dbContext.ClientInsurance
                    .Where(ci => ci.Client.cedula == cedula)
                    .Select(ci => new
                    {
                        InsuranceName = ci.Insurance.name,
                        SumInsured = ci.Insurance.sum_Insured,
                        Premium = ci.Insurance.Premium
                    })
                    .ToListAsync();

                if (insuranceDetails.Any())
                {
                    return new Response()
                    {
                        Code = "00",
                        Message = "Detalles de seguros del cliente obtenidos con éxito",
                        Data = insuranceDetails
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Cliente no encontrado o sin seguros"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al obtener los detalles de seguros del cliente"
                };
            }
        }


        public async Task<Response> GetClientsByInsuranceCodeAsync(int insuranceCode)
        {
            try
            {
                var clientInsurances = await dbContext.ClientInsurance
                    .Include(ci => ci.Client)
                    .Where(ci => ci.Id_Insurance == insuranceCode)
                    .ToListAsync();

                if (clientInsurances.Any())
                {
                    var clientDetailsList = clientInsurances.Select(clientInsurance => new
                    {
                        cedula = clientInsurance.Client.cedula,
                        name = clientInsurance.Client.name,
                        phone = clientInsurance.Client.phone,
                        age = clientInsurance.Client.age
                    }).ToList();

                    return new Response()
                    {
                        Code = "00",
                        Message = "Detalles de los clientes obtenidos con éxito",
                        Data = clientDetailsList
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Seguro no encontrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al obtener los detalles de los clientes"
                };
            }
        }
        public async Task<Response> RemoveInsuranceFromClientAsync(string cedula, string insuranceName)
        {
            try
            {
                var client = await dbContext.Client.FirstOrDefaultAsync(c => c.cedula == cedula);

                if (client != null)
                {
                    var insurance = await dbContext.Insurance.FirstOrDefaultAsync(i => i.name == insuranceName);

                    if (insurance != null)
                    {
                        var clientInsurance = await dbContext.ClientInsurance.FirstOrDefaultAsync(ci =>
                            ci.Id_Client == client.id_client && ci.Id_Insurance == insurance.id);

                        if (clientInsurance != null)
                        {
                            dbContext.ClientInsurance.Remove(clientInsurance);
                            await dbContext.SaveChangesAsync();

                            return new Response
                            {
                                Code = "00",
                                Message = "Seguro eliminado del cliente con éxito"
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                Code = "02",
                                Message = "Seguro no asociado al cliente"
                            };
                        }
                    }
                    else
                    {
                        return new Response
                        {
                            Code = "02",
                            Message = "Seguro no encontrado"
                        };
                    }
                }
                else
                {
                    return new Response
                    {
                        Code = "02",
                        Message = "Cliente no encontrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "02",
                    Message = "Error al eliminar el seguro del cliente"
                };
            }
        }


    }
}
