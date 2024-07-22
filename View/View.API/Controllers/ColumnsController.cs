using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using View.DTO.Databases;
using View.Model.Enteties;
using View.Repository.Columns;
using View.Repository.Tables;

namespace View.API.Controllers
{
    [Route("api/columns")]
    [ApiController]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnRepository _columnRepository;

        public ColumnsController(IColumnRepository columnRepository)
        {
            _columnRepository = columnRepository;
        }


        [SwaggerOperation(Summary = "Retrieves data about column schema using id")]
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var column = await _columnRepository.GetColumnByIdAsync(id);
            if (column == null)
                return NotFound();

            return Ok(column);
        }


        [SwaggerOperation(Summary = "Retrieves data about column schema using name and table schema id")]
        [HttpGet("{name}/tables/{id}"), Authorize]
        public async Task<IActionResult> GetByName(string name, int id)
        {
            var column = await _columnRepository.GetColumnByNameAsync(name, id);
            if (column == null)
                return NotFound();

            return Ok(column);
        }


        [SwaggerOperation(Summary = "Retrieves data about all column schemats that are part of one table schema using table id")]
        [HttpGet("databases/{id}"), Authorize]
        public async Task<IActionResult> GetAll(int id)
        {
            var column = await _columnRepository.GetAllColumnsAsync(id);
            if (column == null)
                return NotFound();

            return Ok(column);
        }


        [SwaggerOperation(Summary = "Allows user to change an existing column schema entity")]
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutById(int id, [FromBody] ColumnDto newColumn)
        {
            if (newColumn == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var column = await _columnRepository.GetColumnByIdAsync(id);
            if (column == null)
                return NotFound();

            column.Name = newColumn.Name;
            column.DataType = newColumn.DataType;
            column.PrimaryKeyStatus = newColumn.PrimaryKeyStatus;
            column.ForeignKeyStatus = newColumn.ForeignKeyStatus;

            var result = await _columnRepository.SaveColumnAsync(column);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [SwaggerOperation(Summary = "Post new column schema entity to database, attaching it to table schema using its id")]
        [HttpPost("items"), Authorize]
        public async Task<IActionResult> PostNewEntity(int id, [FromBody] ColumnDto newColumn)
        {
            if (newColumn == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var column = new ColumnModel
            {
                Name = newColumn.Name,
                DataType = newColumn.DataType,
                PrimaryKeyStatus = newColumn.PrimaryKeyStatus,
                ForeignKeyStatus = newColumn.ForeignKeyStatus,
                Table_ID = id
            };

            var result = await _columnRepository.SaveColumnAsync(column);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [SwaggerOperation(Summary = "Deletes existing column schema entity from database")]
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteExistingEntity(int id)
        {
            var result = await _columnRepository.DeleteColumnAsync(id);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }
    }
}
