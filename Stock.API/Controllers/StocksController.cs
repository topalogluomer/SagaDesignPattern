using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Models;
using System.Threading.Tasks;

namespace Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {

        private readonly AppDbContext _context;

        public StocksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Stocks.ToListAsync());

        }
    }
}
