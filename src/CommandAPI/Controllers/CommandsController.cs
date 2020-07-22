
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;


namespace CommandAPI.Controllers
 {
     using CommandAPI.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context;

        public CommandsController(CommandContext context) => _context = context;

        // GET:  api/commands
        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandItems( )
        {
            return _context.CommandItems;
        }

    }

}


