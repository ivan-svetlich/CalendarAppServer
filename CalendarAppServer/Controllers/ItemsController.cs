﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CalendarAppServer.Data;
using CalendarAppServer.DTOs.Requests;
using CalendarAppServer.DTOs.Responses;
using CalendarAppServer.Models;

namespace CalendarAppServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly CalendarAppContext todoAppContext;
        private readonly UserManager<AppUser> userManager;

        public ItemsController(CalendarAppContext todoAppContext, UserManager<AppUser> userManager)
        {
            this.todoAppContext = todoAppContext;
            this.userManager = userManager;
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<List<TodoItemResponse>>> GetAll()
        //{
        //    try
        //    {
        //        AppUser user = await AuthenticateUser();

        //        if (user != null)
        //        {
        //            var todosQuery = await todoAppContext.TodoItems.Where(todo => todo.UserId == user.Id)
        //                                           .Select(todo => ToResponse(todo)).ToListAsync();

        //            try
        //            {
        //                return todosQuery;
        //            }
        //            catch
        //            {
        //                return Ok("puto");
        //            }


        //        }
        //        else
        //        {
        //            return Unauthorized();
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        return Ok(e);
        //    }
        //}

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<TodoItemResponse>>> GetByInterval(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                AppUser user = await AuthenticateUser();

                if (user != null)
                {
                    var allItems = await todoAppContext.TodoItems.Where(
                                todo => todo.UserId == user.Id &&
                                    todo.DueDate.Date >= startDate.Date && todo.DueDate.Date <= endDate.Date)
                                .Select(todo => ToResponse(todo)).ToListAsync();

                    List<IntervalResponse> response = new List<IntervalResponse>();
                    DateTime date = startDate.Date;
                    while(date <= endDate.Date)
                    {
                        var todos = allItems.Where(todo => todo.DueDate.Date == date).ToList();

                        response.Add(new IntervalResponse
                        {
                            DueDate = date,
                            TodoItems = todos
                        });
                        date = date.AddDays(1);
                    }                  
                    try
                    {
                        return Ok(response);
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }


                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TodoItemResponse>> GetById(int id)
        {
            try
            {
                AppUser user = await AuthenticateUser();

                if (user != null)
                {
                    TodoItem todoItem = todoAppContext.TodoItems
                        .FirstOrDefault(item => item.Id == id && item.UserId == user.Id);

                    if (todoItem != null)
                    {
                        return Ok(ToResponse(todoItem));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TodoItem>> Create([FromBody] AddItemRequest addItemRequest)
        {
            try
            {
                AppUser user = await AuthenticateUser();

                if (user != null)
                {
                    if (addItemRequest != null)
                    {
                        TodoItem todoItem = new TodoItem
                        {
                            User = user,
                            Description = addItemRequest.Description,
                            Completed = false,
                            Removed = false,
                            DueDate = addItemRequest.DueDate,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        };
                        
                        await todoAppContext.AddAsync(todoItem);

                        await todoAppContext.SaveChangesAsync();

                        return Ok(ToResponse(todoItem));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TodoItem>> Update(int id, [FromBody] UpdateItemRequest updateItemRequest)
        {
            try
            {
                AppUser user = await AuthenticateUser();

                if (user != null)
                {
                    if (updateItemRequest != null)
                    {
                        var todoItem = todoAppContext.TodoItems
                            .FirstOrDefault(item => (item.Id == id && item.UserId == user.Id));

                        if(todoItem != null)
                        {
                            todoItem.Description = updateItemRequest.Description;
                            todoItem.Completed = updateItemRequest.Completed;
                            todoItem.Removed = updateItemRequest.Removed;
                        }

                        await todoAppContext.SaveChangesAsync();

                        return Ok(ToResponse(todoItem));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TodoItem>> Delete(long id)
        {
            try
            {
                AppUser user = await AuthenticateUser();

                if (user != null)
                {
                    TodoItem todoItem = todoAppContext.TodoItems
                        .FirstOrDefault(item => item.Id == id && item.UserId == user.Id);

                    if (todoItem != null)
                    {
                        todoAppContext.TodoItems.Remove(todoItem);
                    }

                    await todoAppContext.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<AppUser> AuthenticateUser()
        {
            var identity = User.FindFirst(ClaimTypes.NameIdentifier);

            if (identity == null)
            {
                return null;
            }

            AppUser user = await userManager.FindByIdAsync(identity.Value);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        private static TodoItemResponse ToResponse(TodoItem todoItem)
        {
            return new TodoItemResponse
            {
                Id = todoItem.Id,
                UserId = todoItem.UserId,
                Description = todoItem.Description,
                Completed = todoItem.Completed,
                Removed = todoItem.Removed,
                DueDate = todoItem.DueDate,
                CreatedOn = todoItem.CreatedOn,
                UpdatedOn = todoItem.UpdatedOn
            };
        }
    }
}