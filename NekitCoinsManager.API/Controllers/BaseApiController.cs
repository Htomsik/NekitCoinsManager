using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace NekitCoinsManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IMapper Mapper;

    protected BaseApiController(IMapper mapper)
    {
        Mapper = mapper;
    }
} 