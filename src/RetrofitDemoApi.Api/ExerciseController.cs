using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RetrofitDemoApi.Api
{
    public class Code
    {
        public Guid Value{ get; set; }
        public string Name { get; set; }

        public Code(){}
        public Code(Guid value, string name)
        {
            Value = value;
            Name = name;
        }
    }
    public record CodeResponse(Guid Code, string Name);
    public record ValidationSuccessResponse(string Message, Guid Code);
    public record ErrorResponse(string Error);

    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {

        [ProducesResponseType(400,Type=typeof(ErrorResponse))]
        [ProducesResponseType(200,Type=typeof(CodeResponse))]
        [HttpPost("code/{name}")]
        public ActionResult<CodeResponse> GenerateCode([FromRoute]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }
            var value = Guid.NewGuid();

            using var db = GetDb();
            var collection = GetCollection(db);

            var code = new Code(value, name);
            collection.Insert(code);

            return Ok(new CodeResponse(code.Value, code.Name));
        }

        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(200, Type = typeof(ValidationSuccessResponse))]
        [HttpGet("code/{name}/validate")]
        public ActionResult<ValidationSuccessResponse> Validate([FromRoute] string name, [FromHeader(Name = "X-Custom-Code")]string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                return BadRequest(new ErrorResponse("Required header is missing."));
            }

            if (!Guid.TryParse(header, out var headerResult))
            {
                return BadRequest(new ErrorResponse("Required header value is invalid."));
            }

            using var db = GetDb();
            var collection = GetCollection(db);

            var result = collection.FindOne(x => x.Value == headerResult && x.Name == name);

            if (result is null)
            {
                return BadRequest(new ErrorResponse("Code not found."));
            }

            return Ok(new ValidationSuccessResponse("You have provided valid code.", result.Value));
        }

        private static LiteDatabase GetDb()
        {
            return new LiteDatabase(@"TodosDb.db");
        }

        private static ILiteCollection<Code> GetCollection(LiteDatabase db)
        {
            return db.GetCollection<Code>("codes", autoId: BsonAutoId.Int32);
        }
    }
}
