namespace Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Service;

    [ApiController]
    [Route("api/[controller]")]
    public class StandsController: ControllerBase
    {
        private StandService StandService { get; set; }

        public StandsController(
            StandService standService
        )
        {
            this.StandService = standService;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return this.Ok(await this.StandService.GetAll());
        }
    }
}