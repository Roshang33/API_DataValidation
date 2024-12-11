using Microsoft.AspNetCore.Mvc;
using DataValidation.Data;
using System.Threading.Tasks;
using DataValidation;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ValidationRulesController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ValidationRulesController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpPost("insertvalidationrules")]
    public async Task<IActionResult> InsertValidationRules([FromBody] JsonElement validationRule)
    {

        try
        {
            // Extract RuleName
            string ruleName = validationRule.GetProperty("RuleName").GetString() ?? throw new ArgumentException("RuleName is required.");

            // Extract and convert RuleDescription
            BsonDocument ruleDescription;

            if (validationRule.TryGetProperty("RuleDescription", out var ruleDescriptionElement))
            {
                if (ruleDescriptionElement.ValueKind == JsonValueKind.Object)
                {
                    // Convert to BsonDocument if it's a JSON object
                    ruleDescription = BsonDocument.Parse(ruleDescriptionElement.GetRawText());
                }
                else
                {
                    // Handle non-object cases dynamically
                    ruleDescription = new BsonDocument("Value", ruleDescriptionElement.ToString());
                }
            }
            else
            {
                throw new ArgumentException("RuleDescription is required.");
            }
            // Create the document
            var document = new DocumentModel
            {
                Payload = new ValidationRule
                {
                    RuleName = ruleName,
                    RuleDescription = ruleDescription
                }
            };
            await _context.ValidationRules.InsertOneAsync(document);
            return Ok("Validation rule inserted successfully.");
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict("A validation rule with the same name already exists.");
        }
    }

    [HttpPut("updatevalidationrules/{ruleName}")]
    public async Task<IActionResult> UpdateValidationRules(string ruleName, [FromBody] ValidationRule updatedRule)
    {
        if (updatedRule == null)
        {
            return BadRequest("Updated validation rule is null.");
        }

        var filter = Builders<DocumentModel>.Filter.Eq(r => r.Payload.RuleName, ruleName);
        var update = Builders<DocumentModel>.Update
            .Set(r => r.Payload.RuleDescription, updatedRule.RuleDescription)
            .Set(r => r.Payload.RuleName, updatedRule.RuleName);
        // Add other properties to update as needed

        var result = await _context.ValidationRules.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            return NotFound("Validation rule not found.");
        }

        return Ok("Validation rule updated successfully.");
    }
}
