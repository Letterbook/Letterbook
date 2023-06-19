using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountsController
{
    [HttpGet]
    [Route("{id}/[action]")]
    public void Statuses(int id)
    {
        
    }
    
    [HttpGet]
    [Route("{id}")]
    public void GetAccount(int id)
    {
        
    }
}