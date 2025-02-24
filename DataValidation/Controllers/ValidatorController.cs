

using DataValidation;
using DataValidation.Data;
using DataValidation.Helper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class ValidatorController : ControllerBase
 {
    private readonly MongoDbContext _context;

    public ValidatorController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpPost("validaterule/{ruleName}")]
    public async Task<IActionResult> ExecuteValidation(string ruleName, [FromBody] JsonElement PayloadToValidate)
    {

        try
        {
            var filter = Builders<DocumentModel>.Filter.Eq(r => r.Payload.RuleName, ruleName);
            var document = await _context.ValidationRules.Find(filter).FirstOrDefaultAsync();
            var ruleJson = document.ToJson();
            if (ruleJson == null)
            {
                return NotFound(new { message = $"Rule '{ruleName}' not found" });
            }

            var result = Helper.MapValues(JsonConvert.DeserializeObject<JObject>(JObject.Parse(ruleJson).SelectToken("Payload.RuleDescription.Details")?.ToString()), JsonConvert.DeserializeObject<JObject>(PayloadToValidate.ToString()));

            return Ok(ruleJson);

        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict("A validation rule with the same name already exists.");
        }
    }

}

