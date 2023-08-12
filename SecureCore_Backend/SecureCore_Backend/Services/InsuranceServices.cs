using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureCore_Backend.data;
using SecureCore_Backend.Modelo;
using OfficeOpenXml;

namespace SecureCore_Backend.Services
{
    public class InsuranceServices
    {
        private readonly ApplicationDbContext dbContext;
        public InsuranceServices(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        //Get
        public async Task<Response> GetListInsuranceAsync()
        {
            try
            {
                var insurance = await dbContext.Insurance.ToListAsync();
                if (insurance.IsNullOrEmpty())
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
                    Data = insurance
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al obtener la lista de Seguros"
                };
            }
        }
        public async Task<Response> GetInsuranceByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Debe proporcionar un nombre válido",
                    };
                }

                var insurance = await dbContext.Insurance.Where(a => a.name.Equals(name)).ToListAsync();
                if (insurance.IsNullOrEmpty())
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Seguro no registrado",
                    };
                }
                return new Response()
                {
                    Code = "00",
                    Message = "Seguro generado con exito",
                    Data = insurance
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al obtener el seguro"
                };
            }
        }

        public async Task<Response> GetInsuranceById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "El ID proporcionado es inválido"
                    };
                }

                var insurance = await dbContext.Insurance.Where(a => a.id.Equals(id)).ToListAsync();

                if (insurance.IsNullOrEmpty())
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Seguro no registrado"
                    };
                }

                return new Response()
                {
                    Code = "00",
                    Message = "Seguro generado con éxito",
                    Data = insurance
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al obtener el seguro"
                };
            }
        }

        //Post
        public async Task<Response> CreateInsuranceAsync(Insurance insuranceData)
        {
            try
            {
                if (insuranceData.sum_Insured <= 0 || insuranceData.Premium <= 0)
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Los valores de Sum_Insured y Premium deben ser mayores que cero"
                    };
                }

                var existingInsurance = await dbContext.Insurance.AnyAsync(s => s.name.Equals(insuranceData.name));

                if (!existingInsurance)
                {
                    var insurance = new Insurance
                    {
                        name = insuranceData.name,
                        sum_Insured = insuranceData.sum_Insured,
                        Premium = insuranceData.Premium
                    };

                    dbContext.Insurance.Add(insurance);
                    await dbContext.SaveChangesAsync();

                    return new Response()
                    {
                        Code = "00",
                        Message = "Seguro registrado con éxito",
                        Data = insurance
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "Seguro ya registrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al registrar el seguro"
                };
            }
        }

        public async Task<Response> UploadFileInsurance(IFormFile file)
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

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string name = worksheet.Cells[row, 2]?.Value?.ToString();
                            string Premium = worksheet.Cells[row, 3]?.Value?.ToString();
                            string sum_Insured = worksheet.Cells[row, 4]?.Value?.ToString();

                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(Premium) || string.IsNullOrEmpty(sum_Insured))
                            {
                                return new Response()
                                {
                                    Code = "02",
                                    Message = "Valores de prima, suma asegurada y/o nombre inválidos en la fila " + row
                                };
                            }

                            if (!int.TryParse(Premium, out int prima) || !int.TryParse(sum_Insured, out int suma))
                            {
                                return new Response()
                                {
                                    Code = "02",
                                    Message = "Valores de prima y/o suma inválidos en la fila " + row
                                };
                            }

                            var insurance = new Insurance
                            {
                                name = name,
                                Premium = prima,
                                sum_Insured = suma
                            };

                            var insuranceObj = await dbContext.Insurance.AnyAsync(s => s.name.Equals(insurance.name));
                            if (!insuranceObj)
                            {
                                dbContext.Insurance.Add(insurance);
                                await dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                return new Response()
                                {
                                    Code = "02",
                                    Message = "Seguro ya registrado en la fila " + row
                                };
                            }
                        }

                        return new Response()
                        {
                            Code = "00",
                            Message = "Archivo subido correctamente",
                        };
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

        //Put
        public async Task<Response> UpdateInsuranceAsync(int id, Insurance insurance)
        {
            try
            {
                Insurance insuranceObj = await dbContext.Insurance.FindAsync(id);
                if (insuranceObj != null)
                {
                    if (insurance.sum_Insured <= 0 || insurance.Premium <= 0)
                    {
                        return new Response()
                        {
                            Code = "02",
                            Message = "Los valores de sum_Insured y Premium deben ser mayores que cero"
                        };
                    }

                    insuranceObj.name = insurance.name;
                    insuranceObj.sum_Insured = insurance.sum_Insured;
                    insuranceObj.Premium = insurance.Premium;

                    await dbContext.SaveChangesAsync();

                    return new Response()
                    {
                        Code = "00",
                        Message = "Seguro modificado con exito",
                        Data = insuranceObj
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "El seguro no existe"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Se presentó un error al modificar el seguro"
                };
            }
        }

        public async Task<Response> DeleteInsuranceAsync(int id)
        {
            try
            {
                Insurance insuranceObj = await dbContext.Insurance.FindAsync(id);
                if (insuranceObj != null)
                {
                    dbContext.Insurance.Remove(insuranceObj);
                    await dbContext.SaveChangesAsync();
                    return new Response()
                    {
                        Code = "00",
                        Message = "Seguro eliminado con exito",
                        Data = insuranceObj
                    };
                }
                else
                {
                    return new Response()
                    {
                        Code = "02",
                        Message = "El Seguro no está registrado"
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Code = "02",
                    Message = "Error al eliminar el Seguro"
                };
            }
        }



    }
}
