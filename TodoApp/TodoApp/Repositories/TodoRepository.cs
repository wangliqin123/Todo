using System.Collections.Generic;
using System.Linq;
using TodoApp.Entities;

namespace TodoApp.Repositories
{
    public class TodoRepository
    {
        TodoContext TodoContext;

        public TodoRepository(TodoContext todoContext)
        {
            TodoContext = todoContext;
        }

        public List<Todo> GetList()
        {
            return TodoContext.Todos.ToList();
        }

        public List<Todo> NotDoneList()
        {
            var notDone = from title in TodoContext.Todos
                        where title.IsDone == false
                        select title;
            return notDone.ToList();

            //return TodoContext.Todos.Where(x => x.IsDone == false).ToList();
        }

        public void AddTodo(string title)
        {
            var todo = new Todo()
            {
                Title = title,
                IsDone = false,
                IsUrgent = true
            };

            TodoContext.Todos.Add(todo);
            TodoContext.SaveChanges();
        }

        public void DeleteTodo(int id)
        {
            Todo deletedTodo = TodoContext.Todos.FirstOrDefault(x => x.Id == id);
            TodoContext.Todos.Remove(deletedTodo);
            TodoContext.SaveChanges();
        }

        public Todo GetId(int id)
        {
            return TodoContext.Todos.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateTodo(Todo todo)
        {          
            TodoContext.Todos.Update(todo);
            TodoContext.SaveChanges();
        }
    }
}