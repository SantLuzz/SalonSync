using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Api.ViewModels;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Appointments;

public class AppointmentHandler(AppDbContext context) : IAppointmentHandler
{
    private readonly SystemConfigHandler _systemConfigHandler = new (context);
    private string _userId = string.Empty;
    
    public async Task<Response<AppointmentCreatedDto?>> CreateAsync(GenerateScheduleWithEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);

            if (employee is null)
                return new Response<AppointmentCreatedDto?>(null, 404, "Profissíonal não encontrado!");
            
            var service = await context
                .Services
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.ServiceId);

            if (service is null)
                return new Response<AppointmentCreatedDto?>(null, 404, "Serviço não encontrado!");

            var client = await context
                .Clients
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                    && x.Id == request.ClientId);
           
            if (client is null)
                return new Response<AppointmentCreatedDto?>(null, 404, "Cliente não encontrado!");
            
            var appointments = await context
                .Appointments
                .Where(x => x.UserId == request.UserId
                            && x.EmployeeId == request.EmployeeId
                            && x.Date == request.Date.Date
                            && x.Status == EAppointmentStatus.Free
                            && x.StartTime >= request.StartTime
                            && x.EndTime <= request.EndTime)   
                .OrderBy(x => x.StartTime) 
                .ToListAsync();

            if(!appointments.Any())
                return new Response<AppointmentCreatedDto?>(null, 404, "Desculpe, mas o horário não está mais disponível!");

            AppointmentGroup? group = null;
            
            if (appointments.Count > 1)
            {
                group = new()
                {
                    Date = request.Date,
                    EmployeeId = request.EmployeeId,
                    TotalDuration = service.Duration
                };
                
                await context.AppointmentGroups.AddAsync(group);
                await context.SaveChangesAsync();
            }
            
            foreach (var appointment in appointments)
            {
                appointment.ClientId = request.ClientId;
                appointment.Status = EAppointmentStatus.Scheduled;
                appointment.ServiceId = request.ServiceId;
                appointment.AppointmentGroupId = group?.Id ?? null;
            }

            try
            {
                context.Appointments.UpdateRange(appointments);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (group is not null)
                {
                    context.AppointmentGroups.Remove(group);
                    await context.SaveChangesAsync();
                }
                return new Response<AppointmentCreatedDto?>(null, 500, "(SSA006) Não foi reservar os horários!");
            }
            
            AppointmentCreatedDto appointmentCreatedDto = new ()
            {
                UserId  = request.UserId,
                EmployeeId = request.EmployeeId,
                EmployeeName = employee.Name,
                ClientId = request.ClientId,
                ClientName = client.Name,
                ClientEmail = client.Email,
                ServiceId = request.ServiceId,
                ServiceDescription = service.Name,
                Price = service.Price,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Appointments = appointments
                    .Select(x => new AppointmentDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Date = x.Date,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Status = x.Status,
                        ClientId = x.ClientId,
                        EmployeeId = x.EmployeeId,
                        ServiceId = x.ServiceId,
                        AppointmentGroupId = x.AppointmentGroupId,
                    }).ToList()
            };
            
            return new Response<AppointmentCreatedDto?>(appointmentCreatedDto, message:"Agendamento realizado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<AppointmentCreatedDto?>(null, 500, "(SSA005) Não foi possível criar o agendamento!");
        }
    }

    public async Task<Response<SchedullingClosedDTO?>> FinishAsync(FinishSchedulingRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);

            if (employee is null)
                return new Response<SchedullingClosedDTO?>(null, 404, "Profissíonal não encontrado!");
            
            var service = await context
                .Services
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.ServiceId);

            if (service is null)
                return new Response<SchedullingClosedDTO?>(null, 404, "Serviço não encontrado!");

            List<Appointment> appointments = [];

            if (request.AppointmentGroupId == 0)
            {
                var appointment = await context
                    .Appointments
                    .Where(x => x.UserId == request.UserId
                                && x.EmployeeId == request.EmployeeId
                                && x.Date == request.Date.Date
                                && x.StartTime >= request.StartTime
                                && x.EndTime <= request.EndTime
                    )
                    .OrderBy(x => x.StartTime)
                    .FirstOrDefaultAsync();

                if (appointment is null)
                    return new Response<SchedullingClosedDTO?>(null, 404, "Agendamento não encontrado!");
                
                appointments.Add(appointment);
            }
            else
            {
                appointments = await context
                    .Appointments
                    .Where(x => x.UserId == request.UserId
                                && x.EmployeeId == request.EmployeeId
                                && x.Date == request.Date
                                && x.AppointmentGroupId == request.AppointmentGroupId)
                    .ToListAsync();
            }

            foreach (var appointment in appointments)
                appointment.Status = EAppointmentStatus.Completed;
            
            context.Appointments.UpdateRange(appointments);
            await context.SaveChangesAsync();
            
            return new Response<SchedullingClosedDTO?>(new SchedullingClosedDTO
            {
                UserId = request.UserId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ServiceDuration = service.Duration,
                ServiceValue = service.Price,
                Status = EAppointmentStatus.Completed,
                Appointments = appointments
                    .Select(x => new AppointmentDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Date = x.Date,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Status = x.Status,
                        ClientId = x.ClientId,
                        EmployeeId = x.EmployeeId,
                        ServiceId = x.ServiceId,
                        AppointmentGroupId = x.AppointmentGroupId,
                    }).ToList()
            }, message:"Agendamento finalizado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<SchedullingClosedDTO?>(null, 500, "(SSA009) Não foi possível encerrar o agendamento!");
        }
    }
    
    public async Task<Response<SchedullingClosedDTO?>> CancelAsync(FinishSchedulingRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);

            if (employee is null)
                return new Response<SchedullingClosedDTO?>(null, 404, "Profissíonal não encontrado!");
            
            var service = await context
                .Services
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.ServiceId);

            if (service is null)
                return new Response<SchedullingClosedDTO?>(null, 404, "Serviço não encontrado!");

            List<Appointment> appointments = [];

            if (request.AppointmentGroupId == 0)
            {
                var appointment = await context
                    .Appointments
                    .Where(x => x.UserId == request.UserId
                                && x.EmployeeId == request.EmployeeId
                                && x.Date == request.Date.Date
                                && x.StartTime >= request.StartTime
                                && x.EndTime <= request.EndTime
                    )
                    .OrderBy(x => x.StartTime)
                    .FirstOrDefaultAsync();

                if (appointment is null)
                    return new Response<SchedullingClosedDTO?>(null, 404, "Agendamento não encontrado!");
                
                appointments.Add(appointment);
            }
            else
            {
                appointments = await context
                    .Appointments
                    .Where(x => x.UserId == request.UserId
                                && x.EmployeeId == request.EmployeeId
                                && x.Date == request.Date
                                && x.AppointmentGroupId == request.AppointmentGroupId)
                    .ToListAsync();
            }

            var cancellationHistories = appointments
                .Select(x => new AppointmentCancellationHistory
                {
                    UserId = x.UserId,
                    AppointmentId = x.Id,
                    AppointmentGroupId = x.AppointmentGroupId,
                    CanceledAt = DateTime.Now,
                    CanceledBy = request.UserId,
                    CancellationReason = request.CancellationReason,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ClientId = x.ClientId,
                    Date = x.Date,
                    ServiceId = x.ServiceId,
                    EmployeeId = x.EmployeeId,
                    Status = x.Status
                }).ToList();

            appointments.ForEach(x => x.Status = EAppointmentStatus.Cancelled);
            
            context.Appointments.UpdateRange(appointments);
            await context.CancellationHistories.AddRangeAsync(cancellationHistories);
            await context.SaveChangesAsync();
            
            return new Response<SchedullingClosedDTO?>(new SchedullingClosedDTO
            {
                UserId = request.UserId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ServiceDuration = service.Duration,
                ServiceValue = service.Price,
                Status = EAppointmentStatus.Completed,
                Appointments = appointments
                    .Select(x => new AppointmentDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Date = x.Date,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Status = x.Status,
                        ClientId = x.ClientId,
                        EmployeeId = x.EmployeeId,
                        ServiceId = x.ServiceId,
                        AppointmentGroupId = x.AppointmentGroupId,
                    }).ToList()
            }, message:"Agendamento finalizado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<SchedullingClosedDTO?>(null, 500, "(SSA010) Não foi possível cancelar o agendamento!");
        }
    }
    
    public async Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);
            
            if (employee is null)
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 404,
                    "Não foi possível encontrar o profissional!");

            if(request.StartDate.Date < DateTime.Today) return new Response<EmployeeAvailabilityScheduleDTO?>(null, 404, 
                "A data inicial da abertura de agenda não pode ser menor que o dia atual!");
            
            request.StartDate = request.StartDate.Date;
            request.EndDate = request.EndDate.Date;
            
            if(await CheckForScheduleConflictsAsync(request))
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Já existe uma agenda definida para esse profissional para o período especificado!");

            var employeeVm = new EmployeeScheduleVM
            {
                StartTime =  employee.StartTime ?? new TimeOnly(8, 0),
                EndTime = employee?.EndTime ?? new TimeOnly(18, 0),
                LunchStart = employee?.LunchStart ?? ApiConfiguration.DefaultStartLunchTime,
                LunchEnd = employee?.LunchEnd ?? ApiConfiguration.DefaultEndLunchTime,
            };
            
            var timeRange = SetTimeRange(employeeVm);
            if (!timeRange.Any())
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Não foi possível carregar o intervalo de horas!");

            _userId = request.UserId;
            
            var dateRange = SetDateRange(request.StartDate, request.EndDate);
            if (!dateRange.Any())
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Não foi possível definir as datas para agendamento!");
            
            List<Appointment> appointments = dateRange
                .SelectMany(date => timeRange.Select(x => new Appointment
                {
                    UserId = request.UserId,
                    Date = date,
                    StartTime = x.Key,
                    EndTime = x.Value,
                    EmployeeId = request.EmployeeId,
                    Status = EAppointmentStatus.Free,
                })).ToList();

            await context.Appointments.AddRangeAsync(appointments);
            await context.SaveChangesAsync();

            EmployeeAvailabilityScheduleDTO dto = new()
            {
                Id = employee?.Id ?? request.EmployeeId,
                Name = employee?.Name ?? string.Empty,
                AvailableDays = dateRange,
                CountDays = dateRange.Count,
                UserId = request.UserId
            };

            return new Response<EmployeeAvailabilityScheduleDTO?>(dto, 201, "Agenda criada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<EmployeeAvailabilityScheduleDTO?>(null, 500, "(SSA001) Não foi abrir a agenda do profissional!");
        }
    }
    
    public async Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                    && x.Id == request.EmployeeId);

            if (employee is null)
                return new Response<EmployeeAvailabilitySlotsDTO?>(null, 404, "Profissíonal não encontrado!");
            
            var service = await context
                .Services
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                && x.Id == request.ServiceId);

            if (service is null)
                return new Response<EmployeeAvailabilitySlotsDTO?>(null, 404, "Serviço não encontrado!");

            var now = DateTime.Now;
            bool isToday = request.Date.Date == now.Date;
            
            var appointments = await context
                .Appointments
                .AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.UserId == request.UserId
                            && x.EmployeeId == request.EmployeeId
                            && x.Date == request.Date.Date
                            && x.Status == EAppointmentStatus.Free
                            && (!isToday || x.StartTime >= TimeOnly.FromDateTime(now)))
                .OrderBy(a => a.StartTime) // Ordenação essencial para agrupar consecutivos
                .ToListAsync();

            if (!appointments.Any())
            {
                DateTime? nextDay =  await context.Appointments
                    .Where(a => a.EmployeeId == request.EmployeeId && a.Status == EAppointmentStatus.Free &&
                                a.Date > request.Date)
                    .OrderBy(a => a.Date)
                    .Select(a => (DateTime?)a.Date)
                    .FirstOrDefaultAsync();
                    
                return new Response<EmployeeAvailabilitySlotsDTO?>(new EmployeeAvailabilitySlotsDTO
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    AvailableSlots = [],
                    NextAvaliabeDate = nextDay,
                    UserId = request.UserId
                }, 404, 
                    $"Não existem horários disponíveis para a data informada{(nextDay.HasValue ? $", próxima data disponível {nextDay.Value:dd/MM/yyyy}" : "!")}");
            }
                

            // Definir a duração do serviço em slots
            TimeSpan serviceDuration = TimeSpan.FromMinutes(service.Duration);
            int slotDuration = ApiConfiguration.DefaultTimeByAppointment; // Ajuste conforme necessário (duração fixa de cada slot)
            int requiredSlots = (int)Math.Ceiling(serviceDuration.TotalMinutes / slotDuration);

            Dictionary<TimeOnly, TimeOnly> groupedSlots;

            if (requiredSlots == 1)
                groupedSlots = appointments
                    .ToDictionary(x => x.StartTime, x => x.EndTime);
            else
                groupedSlots = GetConsecutiveAvailableSlots(appointments, requiredSlots);

            var employeeAvailability = new EmployeeAvailabilitySlotsDTO
            {
                Id = request.EmployeeId,
                Name = appointments.First().Employee?.Name ?? string.Empty,
                Date = request.Date,
                UserId = request.UserId,
                AvailableSlots = groupedSlots, // Apenas horários válidos
                NextAvaliabeDate = await context.Appointments
                    .Where(a => a.EmployeeId == request.EmployeeId && a.Status == EAppointmentStatus.Free && a.Date > request.Date)
                    .OrderBy(a => a.Date)
                    .Select(a => (DateTime?)a.Date)
                    .FirstOrDefaultAsync()
            };

            if (!employeeAvailability.AvailableSlots.Any())
                return new Response<EmployeeAvailabilitySlotsDTO?>(employeeAvailability, 404, "Nenhum horário disponível na data atual para o profissional.");

            return new Response<EmployeeAvailabilitySlotsDTO?>(employeeAvailability, message: "Horários encontrados com sucesso!");
        }
        catch (Exception)
        {
            return new Response<EmployeeAvailabilitySlotsDTO?>(null, 500, "(SSA002) Não foi buscar os horários disponíveis do profissional!");
        }
    }
    
    public async Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(GetEmployeesWithOpenScheduleRequest request)
    {
        try
        {
            var appointments = await context.Appointments
                .AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.UserId == request.UserId
                            && x.Date == request.Date
                            && x.Status == EAppointmentStatus.Free)
                .OrderBy(x => x.EmployeeId)
                .ThenBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
            
            var employeesAvailability = appointments
                .GroupBy(x => x.EmployeeId)
                .Select(group => new EmployeeAvailabilitySlotsDTO
                {
                    Id = group.Key,
                    Name = group.First().Employee?.Name ?? string.Empty,
                    Date = request.Date,
                    UserId = request.UserId,
                    AvailableSlots = group.ToDictionary(
                        x => x.StartTime,
                        x => x.EndTime
                    ),
                    NextAvaliabeDate = context.Appointments
                        .AsNoTracking()
                        .Where(a => a.EmployeeId == group.Key
                                    && a.Status == EAppointmentStatus.Free
                                    && a.Date > request.Date)
                        .OrderBy(a => a.Date)
                        .Select(a => (DateTime?)a.Date)
                        .FirstOrDefault()
                })
                .ToList();

            return appointments.Any()
                ? new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(employeesAvailability, message: "Profissíonais encontrados com sucesso!")
                : new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(null, 404, "Nenhum profissional com agenda aberta foi encontrado!");
        }
        catch (Exception)
        {
            return new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(null, 500, "(SSA003) Não foi buscar os profissíonais disponíveis!");
        }
    }
    
    public async Task<Response<List<AvailableDateDto>?>> GetAvailableDatesAsync(GetAvailableDateRequest request)
    {
        try
        {
            var appointments = await context
                .Appointments
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId
                    && x.Status == EAppointmentStatus.Free
                    && x.Date >= DateTime.Today)
                .GroupBy(x => x.Date)
                .ToListAsync();

            if (!appointments.Any() && request.EmployeeId is not null)
            {
                if ((await _systemConfigHandler.GetConfigAsync(request.UserId))?.OpenScheduleAutomatically
                    ?? false)
                {
                    var result = await OpenByEmployeeIdAsync(new OpenAppointmentByEmployeeRequest
                    {
                        UserId = request.UserId,
                        EmployeeId = request?.EmployeeId ?? 0,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(7)
                    });

                    if (!result.IsSuccess)
                        return new Response<List<AvailableDateDto>?>(null, 404,
                            "Não existem agendas abertas e não foi possível abrir agenda para o profissíonal informado!");
                    
                    appointments = await context
                        .Appointments
                        .AsNoTracking()
                        .Where(x => x.UserId == request.UserId
                                    && x.Status == EAppointmentStatus.Free
                                    && x.Date >= DateTime.Today)
                        .GroupBy(x => x.Date)
                        .ToListAsync();
                }
            }
            

            return appointments.Any()
                ? new Response<List<AvailableDateDto>?>(appointments.Select(x => new AvailableDateDto
                {
                    Date = x.Key,
                }).ToList(), message: "Dias recuperados com sucesso!")
                : new Response<List<AvailableDateDto>?>(null,404, "Não existem agendas abertas no momento!");
        }
        catch (Exception)
        {
            return new Response<List<AvailableDateDto>?>(null, 500, "(SSA004) Não foi possível buscar os dias disponíveis!");
        }
    }
    
    public async Task<Response<List<AvailableDateDto>?>> GetAvailableDatesByCompanyCodeAsync(GetAvailableDateByCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code);

            if (company is null)
                return new Response<List<AvailableDateDto>?>(null, 404, "Empresa dona da agenda não encontrada!");
            
            var appointments = await context
                .Appointments
                .AsNoTracking()
                .Where(x => x.UserId == company.UserId
                            && x.Status == EAppointmentStatus.Free
                            && x.Date >= DateTime.Today)
                .GroupBy(x => x.Date)
                .ToListAsync();
            
            //TODO: se caso tiver fechado informar no email da empresa responsável
            
            return appointments.Any()
                ? new Response<List<AvailableDateDto>?>(appointments.Select(x => new AvailableDateDto
                {
                    Date = x.Key,
                }).ToList(), message: "Dias recuperados com sucesso!")
                : new Response<List<AvailableDateDto>?>(null,404, "Não existem agendas abertas no momento, tente novamente mais tarde!");
        }
        catch (Exception)
        {
            return new Response<List<AvailableDateDto>?>(null, 500, "(SSA011) Não foi possível buscar os dias disponíveis!");
        }
    }
    public async Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                && x.Id == request.EmployeeId);
            
            if(employee is null) return new PagedResponse<List<AppointmentDto>?>(null, 404, "Profissional não encontrado!");

            var query = context
                .Appointments
                .AsNoTracking()
                .Include(x => x.Service)
                .Include(x => x.Client)
                .Where(x => x.UserId == request.UserId
                            && x.EmployeeId == employee.Id
                            && x.Date == request.Date.Date);
            
            var appointments = await query
                .ToListAsync();
            
            List<AppointmentDto> appointmentDtos = [];
            AppointmentDto? currentGroup = null;

            foreach (var appointment in appointments)
            {
                if (currentGroup == null || 
                    currentGroup.AppointmentGroupId != appointment.AppointmentGroupId || 
                    appointment.Status == EAppointmentStatus.Free || 
                    currentGroup.EndTime < appointment.StartTime) 
                {
                    // Inicia um novo grupo
                    currentGroup = new AppointmentDto
                    {
                        Id = appointment.Id,
                        UserId = appointment.UserId,
                        Date = appointment.Date,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        Status = appointment.Status,
                        ClientId = appointment.ClientId,
                        EmployeeId = appointment.EmployeeId,
                        ServiceId = appointment.ServiceId,
                        AppointmentGroupId = appointment.AppointmentGroupId,
                        Client = appointment.Client is null ? null : new()
                        {
                            Id = appointment.Client.Id,
                            Name = appointment.Client.Name,
                            UserId = appointment.Client.UserId,
                        },
                        Employee = new()
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            UserId = employee.UserId
                        }
                    };
                    appointmentDtos.Add(currentGroup);
                }
                else
                {
                    // Atualiza o horário final do grupo atual
                    currentGroup.EndTime = appointment.EndTime;
                }
            }
            
            
            return new PagedResponse<List<AppointmentDto>?>(appointmentDtos, message: $"Agenda do profissional para o dia {request.Date:dd/MM/yyyy}!");
        }
        catch (Exception)
        {
            return new PagedResponse<List<AppointmentDto>?>(null, 500, "(SSA007) Não foi possível recuperar a agenda do profissional!");
        }
    }

    public async Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeCodeAsync(GetAppointmentByEmployeeCodeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UniqueCode == request.Code);
            
            if(employee is null) 
                return new PagedResponse<List<AppointmentDto>?>(null, 404, "Profissional não encontrado!");

            var query = context
                .Appointments
                .AsNoTracking()
                .Include(x => x.Service)
                .Include(x => x.Client)
                .Where(x => x.UserId == employee.UserId
                            && x.EmployeeId == employee.Id
                            && x.Date == request.Date.Date);
            
            var appointments = await query
                .ToListAsync();
            
            List<AppointmentDto> appointmentDtos = [];
            AppointmentDto? currentGroup = null;

            foreach (var appointment in appointments)
            {
                if (currentGroup == null || 
                    currentGroup.AppointmentGroupId != appointment.AppointmentGroupId || 
                    appointment.Status == EAppointmentStatus.Free || 
                    currentGroup.EndTime < appointment.StartTime) 
                {
                    // Inicia um novo grupo
                    currentGroup = new AppointmentDto
                    {
                        Id = appointment.Id,
                        UserId = appointment.UserId,
                        Date = appointment.Date,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        Status = appointment.Status,
                        ClientId = appointment.ClientId,
                        EmployeeId = appointment.EmployeeId,
                        ServiceId = appointment.ServiceId,
                        AppointmentGroupId = appointment.AppointmentGroupId,
                        Client = appointment.Client is null ? null : new()
                        {
                            Id = appointment.Client.Id,
                            Name = appointment.Client.Name,
                            UserId = appointment.Client.UserId,
                        },
                        Employee = new()
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            UserId = employee.UserId
                        }
                    };
                    appointmentDtos.Add(currentGroup);
                }
                else
                {
                    // Atualiza o horário final do grupo atual
                    currentGroup.EndTime = appointment.EndTime;
                }
            }
            
            
            return new PagedResponse<List<AppointmentDto>?>(appointmentDtos, message: $"Agenda do profissional para o dia {request.Date:dd/MM/yyyy}!");
        }
        catch (Exception)
        {
            return new PagedResponse<List<AppointmentDto>?>(null, 500, "(SSA008) Não foi possível recuperar a agenda do profissional!");
        }
    }

    #region Private Members
    private async Task<bool> CheckForScheduleConflictsAsync (OpenAppointmentByEmployeeRequest request)
    {
        try
        {
            var conflict = await context
                .Appointments
                .AsNoTracking()
                .AnyAsync(x => x.UserId == request.UserId
                     && x.EmployeeId == request.EmployeeId
                     && x.Date >= request.StartDate.Date && x.Date <= request.EndDate.Date);

            return conflict;
        }
        catch
        {
            return false;
        }
    }

    private Dictionary<TimeOnly, TimeOnly> SetTimeRange(EmployeeScheduleVM employeeTime)
    {
        try
        {
            Dictionary<TimeOnly, TimeOnly> timeRange = [];

            for (TimeOnly hour = employeeTime.StartTime; hour < employeeTime.EndTime; hour = hour.AddMinutes(ApiConfiguration.DefaultTimeByAppointment))
            {
                TimeOnly endInterval = hour.AddMinutes(ApiConfiguration.DefaultTimeByAppointment);
                
                
                if ((hour >= employeeTime.LunchStart && hour < employeeTime.LunchEnd) ||
                    (endInterval > employeeTime.LunchStart && endInterval <= employeeTime.LunchEnd))
                    continue;
                
                timeRange.Add(hour, endInterval > employeeTime.EndTime ? employeeTime.EndTime : endInterval);
            }
            
            return timeRange;
        }
        catch
        {
            return new Dictionary<TimeOnly, TimeOnly>();
        }
    }

    private List<DateTime> SetDateRange(DateTime start, DateTime end)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_userId))
                throw new ArgumentNullException("Informe o UserId para recuperar os dias de trabalho!");
            
            var daysOfWeek = _systemConfigHandler.GetDaysOfWeekAsync(_userId)
                .GetAwaiter()
                .GetResult();
            
            daysOfWeek = daysOfWeek.Any() 
                ? daysOfWeek 
                : ApiConfiguration.DefaultAvailableDays;
            
            List<DateTime> days = days = Enumerable
                .Range(0, (end - start).Days + 1)
                .Select(x => start.AddDays(x).Date)
                .Where(date => daysOfWeek.Contains(date.DayOfWeek))
                .ToList();

            return days;
        }
        catch
        {
            return [];
        }
    }
    
    private Dictionary<TimeOnly, TimeOnly> GetConsecutiveAvailableSlots(List<Appointment> appointments, int requiredSlots)
    {
        var availableSlots = new Dictionary<TimeOnly, TimeOnly>();
        var orderedAppointments = appointments.OrderBy(a => a.StartTime).ToList();

        for (int i = 0; i < orderedAppointments.Count; i++)
        {
            List<Appointment> tempGroup = new() { orderedAppointments[i] };
            TimeOnly expectedEnd = orderedAppointments[i].EndTime;

            for (int j = i + 1; j < orderedAppointments.Count; j++)
            {
                if (orderedAppointments[j].StartTime == expectedEnd) // Verifica se são consecutivos
                {
                    tempGroup.Add(orderedAppointments[j]);
                    expectedEnd = orderedAppointments[j].EndTime;

                    if (tempGroup.Count == requiredSlots) // Se atingiu o necessário, salva e sai do loop
                    {
                        availableSlots.Add(tempGroup.First().StartTime, tempGroup.Last().EndTime);
                        i = j; // Atualiza o índice para continuar depois do último grupo salvo
                        break;
                    }
                }
                else
                {
                    break; // Se não for consecutivo, interrompe a busca
                }
            }
        }

        return availableSlots;
    }
    #endregion
}