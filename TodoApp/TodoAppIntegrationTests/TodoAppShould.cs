using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TodoApp;
using TodoApp.Entities;
using TodoApp.Repositories;
using Xunit;
using System.Linq;

namespace TodoAppIntegrationTests
{
    public class TodoAppShould
    {
        private readonly TestServer Server;
        private readonly HttpClient Client;

        public TodoAppShould()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        [Fact]
        public async Task ReturnNotFoundStatusWithWrongRoute()
        {
            var response = await Client.GetAsync("/tod");
            string responseJson = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddNewTodo()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodosAddTestDb")
                .Options;

            using (var dbContext = new TodoContext(options))
            {
                var todoRepository = new TodoRepository(dbContext);

                dbContext.Todos.Add(new Todo()
                {
                    Title = "make lunch",
                });
                dbContext.SaveChanges();

                string expected = "make lunch";
                var todo = await dbContext.Todos
                    .FirstOrDefaultAsync(x => x.Title.Equals("make lunch"));
                Assert.Equal(expected, todo.Title);
            }
        }

        [Fact]
        public void DeleteTodo()
        {
            var options = new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase(databaseName: "TodosDelTestDb").Options;

            using (var dbContext = new TodoContext(options))
            {
                var todoRepository = new TodoRepository(dbContext);

                dbContext.Todos.Add(new Todo()
                {
                    Id = 100,
                });
                dbContext.SaveChanges();

                var deletedTodo = dbContext.Todos.FirstOrDefault(x => x.Id.Equals(100));
                todoRepository.DeleteTodo(deletedTodo.Id);
                dbContext.SaveChanges();

                var expected = 0;
                var numberOfTodos = dbContext.Todos.Count();
                Assert.Equal(expected, numberOfTodos);
            }
        }

        [Fact]
        public async Task UpdateTodo()
        {
            var options = new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase(databaseName: "TodosUpdateTestDb").Options;

            using (var dbContext = new TodoContext(options))
            {
                var todoRepository = new TodoRepository(dbContext);

                dbContext.Todos.Add(new Todo()
                {
                    Id = 99,
                    Title = "make breakfast",
                    IsDone = false,
                    IsUrgent = true
                });
                dbContext.SaveChanges();



                var updateTodo = dbContext.Todos.FirstOrDefault(x => x.Id.Equals(99));
                updateTodo.Title = "make supper";
                todoRepository.UpdateTodo(updateTodo);
                dbContext.SaveChanges();

                string expected = "make supper";
                var todo = await dbContext.Todos.FirstOrDefaultAsync(x => x.Id.Equals(99));
                Assert.Equal(expected, todo.Title);
            }
        }

        [Fact]
        public void DeleteTodoWithAddTwoElementAndRemainsOne()
        {
            var options = new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase(databaseName: "TodosAddTwoTestDb").Options;

            using (var dbContext = new TodoContext(options))
            {
                var todoRepository = new TodoRepository(dbContext);

                dbContext.Todos.Add(new Todo()
                {
                    Id = 100,
                });
                dbContext.SaveChanges();

                dbContext.Todos.Add(new Todo()
                {
                    Id = 101,
                });
                dbContext.SaveChanges();

                var deletedTodo = dbContext.Todos.FirstOrDefault(x => x.Id.Equals(100));
                todoRepository.DeleteTodo(deletedTodo.Id);
                dbContext.SaveChanges();

                var expected = 1;
                var numberOfTodos = dbContext.Todos.Count();
                Assert.Equal(expected, numberOfTodos);
            }
        }
    }
}