using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpcService;
using ToDoList.Data;
using ToDoList.Models.ToDoGrpcService.Models;

namespace ToDoList.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _dbContext;

        public ToDoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

            var todoItem = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
            };

            await _dbContext.AddAsync(todoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = todoItem.ID,
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(item => item.ID == request.Id);

            if (todoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = todoItem.ID,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    Status = todoItem.ToDoStatus
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var todoItems = await _dbContext.TodoItems.ToListAsync();

            foreach (var todo in todoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = todo.ID,
                    Title = todo.Title,
                    Description = todo.Description,
                    Status = todo.ToDoStatus
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateTodo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

            var toDoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(item => item.ID == request.Id);

            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.Status;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = toDoItem.ID,
            });

        }

        public override async Task<DeleteToDoResponse> DeleteTodo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(item => item.ID == request.Id);

            if (todoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id : {request.Id}"));

            _dbContext.Remove(todoItem);

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = todoItem.ID,
            });

        }

    }
}
