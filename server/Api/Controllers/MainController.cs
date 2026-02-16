using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class MainController(MainService service) : ControllerBase
{
    
}