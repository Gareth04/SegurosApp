using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;

namespace SecureCore_Backend.Services
{
    public class ClientServices
    {
        private readonly ApplicationDbContext dbContext;

        public ClientServices(ApplicationDbContext context)
        {
            dbContext = context;
        }

        //Get

        public async Task<Response> GetClientList()
        {
            try
            {
                var client = await dbContext.Client.ToListAsync();

                if (client.IsNullOrEmpty())
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "No existen datos para presentar",
                    };
                }

                return new Response()
                {
                    Code = "00",
                    Message = "Lista generada con exito",
                    Data = client
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al obtener la lista de clientes"
                };
            }
        }

        public async Task<Response> CreateClient(Client clientInput)
        {
            try
            {
                var existingClient = await dbContext.Client.FirstOrDefaultAsync(c => c.cedula == clientInput.cedula);

                if (existingClient != null)
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "El cliente ya está registrado"
                    };
                }

                var newClient = new Client
                {
                    cedula = clientInput.cedula,
                    name = clientInput.name,
                    phone = clientInput.phone,
                    age = clientInput.age
                };

                dbContext.Client.Add(newClient);
                await dbContext.SaveChangesAsync();

                return new Response()
                {
                    Code = "00",
                    Message = "Cliente registrado con éxito",
                    Data = newClient
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al registrar el cliente"
                };
            }
        }

        public async Task<Response> UploadFileClient(IFormFile file)
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
                    ExcelWorksheet worksheet = null!;
                    foreach (var sheet in package.Workbook.Worksheets)
                    {
                        worksheet = sheet;
                        break;
                    }

                    if (worksheet != null)
                    {
                        try
                        {
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                string cedula = worksheet.Cells[row, 2].Value!.ToString()!;
                                string name = worksheet.Cells[row, 3].Value!.ToString()!;
                                string phone = worksheet.Cells[row, 4].Value!.ToString()!;
                                string ageStr = worksheet.Cells[row, 5].Value!.ToString()!;

                                int age;
                                if (!int.TryParse(ageStr, out age))
                                {
                                    return new Response()
                                    {
                                        Code = "02",
                                        Message = "Valor de edad inválido"
                                    };
                                }

                                Client client = new Client
                                {
                                    cedula = cedula,
                                    name = name,
                                    phone = phone,
                                    age = age
                                };

                                var existingClient = await dbContext.Client.AnyAsync(c => c.cedula.Equals(client.cedula));
                                if (!existingClient)
                                {
                                    dbContext.Client.Add(client);
                                    await dbContext.SaveChangesAsync();
                                }
                                else
                                {
                                    return new Response()
                                    {
                                        Code = "02",
                                        Message = "Cliente ya registrado"
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
                    Message = "Archivo incorrecto"
                };
            }
        }

        public async Task<Response> UpdateClient(int id, Client clientUpdate)
        {
            try
            {
                var client = await dbContext.Client.FirstOrDefaultAsync(a => a.id_client == id);

                if (client != null)
                {
                    if (clientUpdate.cedula == client.cedula)
                    {
                        client.name = clientUpdate.name;
                        client.phone = clientUpdate.phone;
                        client.age = clientUpdate.age;

                        dbContext.Client.Update(client);
                        await dbContext.SaveChangesAsync();

                        return new Response()
                        {
                            Code = "00",
                            Message = "Cliente modificado con éxito",
                            Data = client
                        };
                    }
                    else
                    {
                        return new Response()
                        {
                            Code = "02",
                            Message = "No se puede cambiar el número de cédula"
                        };
                    }
                }

                return new Response()
                {
                    Code = "02",
                    Message = "El cliente no existe"
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al modificar al cliente"
                };
            }
        }

        public async Task<Response> DeleteClient(int id)
        {
            try
            {
                Client clientObj = await dbContext.Client.FindAsync(id);
                if (clientObj != null)
                {
                    dbContext.Client.Remove(clientObj);
                    await dbContext.SaveChangesAsync();

                    return new Response()
                    {
                        Code = "00",
                        Message = "Cliente eliminado con exito",
                        Data = clientObj
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "El Cliente no está registrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al eliminar el cliente"
                };
            }
        }

    }
}
