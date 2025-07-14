using AutoMapper;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Business.Services.Concrete;

public abstract class BaseService
{
    protected readonly IMapper Mapper;
    protected readonly IConfiguration Config;
    protected readonly ICurrentUserService CurrentUserService;
    protected BaseService(
        IMapper mapper,
        IConfiguration config,
        ICurrentUserService currentUserService)
    {
        Mapper = mapper;
        Config = config;
        CurrentUserService = currentUserService;
    }

}
