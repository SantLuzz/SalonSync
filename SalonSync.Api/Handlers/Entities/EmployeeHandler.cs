using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Data;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Entities;

public class EmployeeHandler(AppDbContext context) : IEmployeeHandler
{
    public async Task<Response<Employee?>> CreateAsync(CreateEmployeeRequest request)
    {
        try
        {
            if(await MethodsExtensions.CheckFieldInUse(context.Employees, 
                   x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)
                   && x.UserId == request.UserId))
                return new Response<Employee?>(null, 400, "O e-Mail informado já está vinculado a outro funcionário!");
            
            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Position = request.Position,
                CreatedAt = DateTime.Now,
                Admission = request.Admission,
                Commission = request.Commission,
                UserId = request.UserId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                LunchStart = request.LunchStart ?? ApiConfiguration.DefaultStartLunchTime,
                LunchEnd = request.LunchEnd ?? ApiConfiguration.DefaultEndLunchTime,
            };
            
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            return new Response<Employee?>(employee, 201, "Funcionário criado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE001) Não foi possível cadastrar o funcionário!");
        }
    }

    public async Task<Response<Employee?>> UpdateAsync(UpdateEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);

            if (employee is null)
                return new Response<Employee?>(null, 404, "Funcionário não encontrado!");

            if (!employee.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if(await MethodsExtensions.CheckFieldInUse(context.Employees, 
                       x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)
                            && x.UserId == request.UserId))
                    return new Response<Employee?>(null, 400, "O e-Mail informado já está vinculado a outro Funcionário!");
            }

            employee.Name = request.Name;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.Position = request.Position;
            employee.Admission = request.Admission;
            employee.Commission = request.Commission;
            employee.UserId = request.UserId;
            employee.UpdatedAt = DateTime.Now;
            employee.StartTime = request.StartTime;
            employee.EndTime = request.EndTime;
            employee.LunchStart = request.LunchStart ?? ApiConfiguration.DefaultStartLunchTime;
            employee.LunchEnd = request.LunchEnd ?? ApiConfiguration.DefaultEndLunchTime;

            context.Employees.Update(employee);
            await context.SaveChangesAsync();

            return new Response<Employee?>(employee, message: "Funcionário alterado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE002) Não foi possível alterar o funcionário!");
        }
    }

    public async Task<Response<Employee?>> UpdateImageAsync(UpdateImageEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);
            
            if (employee is null)
                return new Response<Employee?>(null, 404, "Funcionário não encontrado!");
            
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            
            var data = new Regex(@"^data:image/[a-z]+;base64,")
                .Replace(request.Base64Image, string.Empty);
            
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"{ApiConfiguration.FileFolder}{fileName}", bytes);
            }
            catch (Exception)
            {
                return new Response<Employee?>(null, 500, "(SSE004) Não foi possível gravar a imagem do funcionário!");
            }

            employee.Image = ApiConfiguration.UrlFiles + fileName;
            employee.UpdatedAt = DateTime.Now;
            
            context.Employees.Update(employee);
            await context.SaveChangesAsync();
            
            return new Response<Employee?>(employee, message: "Imagem alterada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE003) Não foi possível alterar a imagem do Funcionário!");
        }
    }

    public async Task<Response<Employee?>> DeleteAsync(DeleteEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);
            
            if (employee is null)
                return new Response<Employee?>(null, 404, "Funcionário não encontrado!");
            
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
            
            return new Response<Employee?>(employee, message: "Funcionário removido com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE005) Não foi possível excluir o funcionário!");
        }
    }

    public async Task<Response<Employee?>> GenerateUniqueCodeAsync(GenerateUniqueEmployeeCodeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);
            if (employee is null)
                return new Response<Employee?>(null, 404, "Funcionário não encontrado!");

            employee.UniqueCode = Guid.NewGuid();
            
            context.Employees.Update(employee);
            await context.SaveChangesAsync();

            return new Response<Employee?>(employee, message: "Código gerado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE008) Não foi possível gerar o código de visualização do funcionário!");
        }
    }

    public async Task<Response<Employee?>> GetByIdAsync(GetEmployeeByIdRequest request)
    {
        DateTime startOfWeek;
        DateTime endOfWeek;
        try
        {
            var today = DateTime.UtcNow.Date;
            startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            endOfWeek = startOfWeek.AddDays(6);
        }
        catch 
        {
           startOfWeek = DateTime.Today;
           endOfWeek = DateTime.Today.AddDays(6);
        }
        
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);

            if (employee is null)
                return new Response<Employee?>(null, 404, "Funcionário não encontrado!");
            
            employee.Appointments = await context
                .Appointments
                .AsNoTracking()
                .Where(a => a.EmployeeId == employee.Id
                    && a.UserId == request.UserId
                    && a.Date.Date >= startOfWeek && a.Date.Date <= endOfWeek)
                .ToListAsync();
            
            
            return new Response<Employee?>(employee, message:"Funcionário encontrado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Employee?>(null, 500, "(SSE006) Não foi possível recuperar o funcionário!");
        }
    }

    public async Task<PagedResponse<List<Employee>?>> GetAllAsync(GetAllEmployeesRequest request)
    {
        try
        {
            var query = context
                .Employees
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            var employees = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Employee>?>(employees, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Employee>?>(null, 500, "(SSE007) Não foi possível recuperar os funcionários!");
        }
    }
}