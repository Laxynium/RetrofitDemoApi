using System.ComponentModel;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace RetrofitDemoApi.Api
{
    public record CreateTodo(string Text, bool Completed);

    public record UpdateTodo(string Text, bool Completed);


    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] int offset = 0, [FromQuery] int? limit = null)
        {
            offset = offset < 0 ? 0 : offset;
            limit = limit < 1 ? null : limit;

            using var db = GetDb();

            var collection = GetCollection(db);

            var result = collection.Query().Skip(offset);
            if (limit is { })
            {
                result = result.Limit(limit.Value);
            }

            var todos = result.ToList();

            return Ok(todos);
        }

        /// <summary>
        /// Creates a new TodoItem
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] CreateTodo request)
        {
            using var db = GetDb();

            var collection = GetCollection(db);

            var todo = new Todo(request.Text, request.Completed);

            collection.Insert(todo);

            return Ok(todo);
        }


        /// <summary>
        /// Update a specific TodoItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateTodo request)
        {
            using var db = GetDb();

            var collection = GetCollection(db);

            var todo = collection.FindById(id);

            if (todo is null)
            {
                return BadRequest(new {error="Todo not found"});
            }

            if (request.Text == "Wrong")
            {
                return BadRequest(new { error = "Invalid todo name" });
            }

            todo.Completed = request.Completed;
            todo.Text = request.Text;

            collection.Update(todo);

            return Ok(todo);
        }

        /// <summary>
        /// Deletes a specific TodoIte
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            using var db = GetDb();

            var collection = GetCollection(db);

            var todo = collection.FindById(id);

            if (todo is null)
            {
                return BadRequest(new { error = "Todo not found" });
            }

            collection.Delete(todo.Id);

            return NoContent();
        }

        private static LiteDatabase GetDb()
        {
            return new LiteDatabase(@"TodosDb.db");
        }

        private static ILiteCollection<Todo> GetCollection(LiteDatabase db)
        {
            return db.GetCollection<Todo>("todos", autoId: BsonAutoId.Int32);
        }
    }
}