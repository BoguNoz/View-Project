using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using View.DTO.Databases;
using View.Model.Enteties;
using View.Repository.Databases;
using View.Repository.Tables;

namespace View.API.Controllers
{
    [Route("api/tables/")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;

        public TablesController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }


        [SwaggerOperation(Summary = "Retrieves data about table schema using id")]
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _tableRepository.GetTableByIdAsync(id);
            if (table == null)
                return NotFound();

            return Ok(table);
        }


        [SwaggerOperation(Summary = "Retrieves data about table schema using name and database schema id")]
        [HttpGet("{name}/databases/{id}"), Authorize]
        public async Task<IActionResult> GetByName(string name, int id)
        {
            var table = await _tableRepository.GetTableByNameAsync(name, id);
            if (table == null)
                return NotFound();

            return Ok(table);
        }


        [SwaggerOperation(Summary = "Retrieves data about all table schemats that are part of one database schema using database id")]
        [HttpGet("databases/{id}"), Authorize]
        public async Task<IActionResult> GetAll(int id)
        {
            var table = await _tableRepository.GetAllTableAsync(id);
            if (table == null)
                return NotFound();

            return Ok(table);
        }


        [SwaggerOperation(Summary = "Allows user to change an existing table schema entity")]
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutById(int id, [FromBody] TablesDto newTable)
        {
            if (newTable == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var table = await _tableRepository.GetTableByIdAsync(id);
            if (table == null)
                return NotFound();

            table.Name =  newTable.Name;

            var result = await _tableRepository.SaveTableAsync(table);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [SwaggerOperation(Summary = "Post new table schema entity to database, attaching it to database schema using its id")]
        [HttpPost("items"), Authorize]
        public async Task<IActionResult> PostNewEntity(int id, [FromBody] TablesDto newTable)
        {
            if (newTable == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var table = new TableModel {

                Name = newTable.Name,
                Database_ID = id,
                TableColumns = new List<ColumnModel>(),
                TableRelations = new List<TableRelationModel>(),
                InRelationWithTable = new List<TableRelationModel>()
            };

            var result = await _tableRepository.SaveTableAsync(table);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [SwaggerOperation(Summary = "Deletes existing table schema entity from database")]
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteExistingEntity(int id)
        {
            var result = await _tableRepository.DeleteTableAsync(id);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }
    }
}
