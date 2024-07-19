using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using View.DTO.Databases;
using View.Model.Enteties;
using View.Repository.Databases;

namespace View.API.Controllers
{
    [Route("api/databases/")]
    [ApiController]
    public class DatatabasesController : ControllerBase
    {
        private readonly IDatabaseRepository _databaseRepository;

        private readonly UserManager<ApplicationUserModel> _userManager;

        public DatatabasesController(IDatabaseRepository databaseRepository, UserManager<ApplicationUserModel> userManager)
        {
            _databaseRepository = databaseRepository;
            _userManager = userManager;
        }


        [HttpGet("id={id}"), Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var database = await _databaseRepository.GetDatabaseByIdAsync(id);
            if (database == null)
                return NotFound();

            return Ok(database);
        }


        [HttpGet("name={name}"), Authorize]
        public async Task<IActionResult> GetByName(string name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var database = await _databaseRepository.GetDatabaseByNameAsync(name, user.Id);
            if(database == null)
                return NotFound();

            return Ok(database);
        }


        [HttpGet, Authorize]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var database = await _databaseRepository.GetAllDatabasesAsync(user.Id);
            if (database == null)
                return NotFound();

            return Ok(database);
        }


        [HttpPut("id={id}"), Authorize]
        public async Task<IActionResult> PutById(int id, [FromBody] DatabaseDto newDatabase)
        {
            if (newDatabase == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var database = await _databaseRepository.GetDatabaseByIdAsync(id);
            if(database == null)
                return NotFound();

            database.Name = newDatabase.Name;
            database.Description = newDatabase.Description;
            database.CreationDate = DateTime.Now;

            var result = await _databaseRepository.SaveDatabaseAsync(database);
            if(!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [HttpPost, Authorize]
        public async Task<IActionResult> PostNewEntity([FromBody] DatabaseDto newDatabase)
        {
            if (newDatabase == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var database = new DatabaseModel
            {
                Name = newDatabase.Name,
                Description = newDatabase.Description,
                CreationDate = DateTime.Now,
                User_ID = user.Id
            };

            var result = await _databaseRepository.SaveDatabaseAsync(database);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }
    }
}
