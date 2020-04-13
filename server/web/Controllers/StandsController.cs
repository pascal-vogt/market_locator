namespace Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Service;

    [ApiController]
    [Route("api/[controller]")]
    public class StandsController: ControllerBase
    {
        private StandService StandService { get; set; }
        private IAuthorizationService AuthorizationService { get; set; }

        public StandsController(
            StandService standService,
            IAuthorizationService authorizationService
        )
        {
            this.StandService = standService;
            this.AuthorizationService = authorizationService;
        }
        
        [HttpGet]
        [Authorize(Policy = "STAND_R")]
        public async Task<ActionResult> GetAll()
        {
            // TODO: we can call this manually and pass a context as the second parameter (for example a stand)
            //       we should probably use this for things like updating a stand
            /*if (!(await this.AuthorizationService.AuthorizeAsync(User, stand, "STAND_R")).Succeeded)
            {
                return this.Unauthorized();
            }*/
            
            return this.Ok(await this.StandService.GetAll());
        }
    }
}