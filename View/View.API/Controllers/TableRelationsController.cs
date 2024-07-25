using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using View.DTO.Databases;
using View.Model.Enteties;
using View.Repository.Nowy_folder;
using View.Repository.Tables;

namespace View.API.Controllers
{
    [Route("/relations/")]
    [ApiController]
    public class TableRelationsController : ControllerBase
    {
        private readonly ITableReltionRepository _relationRepository;

        private readonly UserManager<ApplicationUserModel> _userManager;

        public TableRelationsController(ITableReltionRepository tableRelationRepository, UserManager<ApplicationUserModel> userManager)
        {
            _relationRepository = tableRelationRepository;
            _userManager = userManager;
        }


        [SwaggerOperation(Summary = "Post new table relation entity to database")]
        [HttpPost("tables/{tableId}/tables{relationId}/databases/{databaseId}"), Authorize]
        public async Task<IActionResult> PostNewEntity(int tableId, int relationId, string databaseId)
        {

            var relation = new TableRelationModel
            {
                Table_ID = tableId,
                Relation_ID = relationId,
            };

            var result = await _relationRepository.SaveTableRelationAsync(relation);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }


        [SwaggerOperation(Summary = "Deletes existing table relation entity from database")]
        [HttpDelete("tables/{tableId}/tables/{relationId}"), Authorize]
        public async Task<IActionResult> DeleteExistingEntity(int tableId, int relationId)
        {
            var result = await _relationRepository.DeleteTableRelationAsync(tableId,relationId);
            if (!result.Status)
                throw new Exception(result.Message);

            return Ok(result);
        }
    }
}
