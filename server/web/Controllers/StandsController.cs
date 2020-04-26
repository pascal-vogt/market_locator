namespace Web.Controllers
{
    using System.Threading.Tasks;
    using Database.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Service;

    [ApiController]
    [Route("api/[controller]")]
    public class StandsController: ControllerBase
    {
        private StandService StandService { get; set; }
        private IAuthorizationService AuthorizationService { get; set; }
        private GoogleDocImportService GoogleDocImportService { get; set; }

        public StandsController(
            StandService standService,
            GoogleDocImportService googleDocImportService,
            IAuthorizationService authorizationService
        )
        {
            this.StandService = standService;
            this.AuthorizationService = authorizationService;
            this.GoogleDocImportService = googleDocImportService;
        }
        
        [HttpGet]
        [Authorize(Policy = "STAND_R")]
        public async Task<ActionResult> GetAll()
        {
            return this.Ok(await this.GoogleDocImportService.GetStands());
            //return this.Ok(await this.StandService.GetAll());
        }
        
        [HttpGet]
        [Route("{standId}")]
        public async Task<ActionResult> GetById(long standId)
        {
            var stand = await this.StandService.GetById(standId);
            if (!(await this.AuthorizationService.AuthorizeAsync(User, stand, "STAND_R")).Succeeded)
            {
                return this.Unauthorized();
            }
            
            return this.Ok(stand);
        }
        
        [HttpDelete]
        [Route("{standId}")]
        public async Task<ActionResult> DeleteById(long standId)
        {
            var stand = await this.StandService.GetById(standId);
            if (!(await this.AuthorizationService.AuthorizeAsync(User, stand, "STAND_D")).Succeeded)
            {
                return this.Unauthorized();
            }
            
            return this.NoContent();
        }
        
        [HttpPut]
        [Route("{standId}")]
        public async Task<ActionResult> UpdateById(long standId, [FromBody] Stand stand)
        {
            var standFromDb = await this.StandService.GetByIdForUpdate(standId);
            if (!(await this.AuthorizationService.AuthorizeAsync(User, standFromDb, "STAND_U")).Succeeded)
            {
                return this.Unauthorized();
            }

            standFromDb = await this.StandService.Update(standFromDb, stand);
            
            return this.Ok(standFromDb);
        }
        
        [HttpPost]
        [Authorize(Policy = "STAND_C")]
        public async Task<ActionResult> Create([FromBody] Stand stand)
        {
            stand = await this.StandService.Create(stand);
            
            return this.Ok(stand);
        }
    }
}