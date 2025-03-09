

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
using static System.Runtime.InteropServices.JavaScript.JSType;

[ApiController]
[Route("api/[controller]")]
public class ValidatorController : ControllerBase
 {
    private readonly MongoDbContext _context;
    private readonly DslValidator _dslValidator;

    public ValidatorController(MongoDbContext context, DslValidator dslValidator)
    {
        _context = context;
        _dslValidator = dslValidator;
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

            //foreach (var item in result)
            //{
            //    bool isValid = _dslValidator.ExecuteCustomValidation(item.Key, item.Value);
            //}
            //return Ok(new { message = "Validation passed!" });

            var validationTasks = result
                .Select(async kvp =>
                {
                    bool isValid = await _dslValidator.ExecuteCustomValidationAsync(kvp.Key, kvp.Value);

                    return isValid ? null : $"Validation failed for {kvp.Key}.";
                });

            var results = await Task.WhenAll(validationTasks);
            var errors = results.Where(error => error != null).ToList();
            if (errors.Any())
            {
                return BadRequest(new { message = "Validation failed", errors });
            }

            return Ok(new { message = "Validation passed!" });

        }
        catch (MongoWriteException ex)
        {
            return Conflict($"{ex}");
        }
    }

}

