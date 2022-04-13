using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_diary.Api.Controllers;
using my_diary.Api.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace my_diary.Api.Tests
{
    public class SetupFixture : IDisposable
    {
        public DbContextOptions<MainDbContext> options = new DbContextOptionsBuilder<MainDbContext>().UseInMemoryDatabase("diaryDbInMem").Options;
        public SetupFixture()
        {
            using var context = new MainDbContext(options);
            context.Users.AddRange(new User[]
            { 
                new User { Email = "user1@mail.com", Id = "user1Id", UserName = "user1", Password = "abc.123", Entries = new Entry[]
                    {
                       new Entry{Id = "one", LastUpdated = DateTimeOffset.UtcNow, Title = "Note one", Text = "This is the first note."},
                       new Entry{Id = "two", LastUpdated = DateTimeOffset.UtcNow, Title = "Note two", Text = "This is the second note"},
                       new Entry{Id = "three", LastUpdated = DateTimeOffset.UtcNow, Title = "Note five", Text = "This is the third note"},
                       new Entry{Id = "four", LastUpdated = DateTimeOffset.UtcNow, Title = "Note five", Text = "This is the fourth note"},
                    } 
                },
                new User{ Email = "user2@mail.com", Id = "user2Id", UserName = "user2", Password = "abc.123"}

            });

            context.SaveChanges();
        }
        public void Dispose() { }
    }

    public class EntriesControllerTests : IClassFixture<SetupFixture>
    {
        private readonly DbContextOptions<MainDbContext> _options;
        private HttpContext httpContext = new DefaultHttpContext();
        public EntriesControllerTests(SetupFixture fixture)
        {
            _options = fixture.options;
            httpContext.Request.Headers["Authorization"] = "Bearer xyz";
            httpContext.Items["User"] = "user1Id";
        }

        [Fact]
        public void CreateEntry_AddEntry_ReturnsAddedEntry()
        {

            using var context = new MainDbContext(_options);

            ////Arrange
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            var entry = new EntryDto
            {
                Title = "Note five",
                Text = "This is the fifth note"
            };
            //Act
            var result = entriesController.CreateEntry(entry);
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateEntry_AddEntry_IncreasesEntriesInDbByOne()
        {
            using var context = new MainDbContext(_options);
            //Arrange
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var entry = new EntryDto
            {
                Title = "Note six",
                Text = "This is the sixth note"
            };

            var user = context.Users.First(u => u.Id == "user1Id");
            //Act
            var entriesBeforeAdd = user.Entries.ToList().Count;
            var result = entriesController.CreateEntry(entry);
            var entriesAfterAdd = user.Entries.ToList().Count;
            var diff = entriesAfterAdd - entriesBeforeAdd;

            //Assert 
            var createdEntry = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(1, diff);
            Assert.Contains((Entry)createdEntry.Value, user.Entries);
        }

        [Fact]
        public void CreateEntry_InvalidEntry_ReturnsBadRequest()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            var result = entriesController.CreateEntry(new Entry());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAll_NoEntriesExist_ReturnsErrorCode404()
        {
            using var context = new MainDbContext(_options);

            HttpContext httpContext2 = new DefaultHttpContext();
            httpContext2.Request.Headers["Authorization"] = "Bearer wxyz";
            httpContext2.Items["User"] = "user2Id";

            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext2
                }
            }; 
            var response = entriesController.GetAll();
            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        public void GetAll_EntriesExist_ReturnsOkResult()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            }; 

            var result = entriesController.GetAll();
            var okObjResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var entries = okObjResult.Value;
            Assert.IsType<List<Entry>>(entries);
            Assert.NotEmpty((IEnumerable)entries);

        }

        [Fact]
        public void GetOne_UseWrongId_ReturnsBadRequest()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            var result = entriesController.GetOne("test");
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetOne_ProvideValidId_ReturnEntry()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var entry = entriesController.GetOne("three");
            var returned = Assert.IsAssignableFrom<OkObjectResult>(entry);
            var item = Assert.IsType<Entry>(returned.Value);
            Assert.Equal("three", item.Id);
        }

        [Fact]
        public void UpdateEntry_PassInvalidParams_ReturnsBadRequest()
        {
            using var context = new MainDbContext(_options);

            var testEntry = new Entry
            {
                Title = "Note one update",
                Text = "Updated the first note"
            };

            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            //Invalid Id, valid object
            var result = entriesController.UpdateEntry(null, testEntry);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            //Valid Id, invalid object
            var result2 = entriesController.UpdateEntry("one", new Entry());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result2);


        }

        [Fact]
        public void UpdateEntry_PassValidParams_ReturnsOk()
        {
            using var context = new MainDbContext(_options);

            var testEntry = new EntryDto
            {
                Title = "Note two update",
                Text = "Updated the second note"
            };

            var entriesController = new EntriesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var result = entriesController.UpdateEntry("two", testEntry);
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void UnauthorizedUser_CannotAccessEndpoints()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context);

            Assert.Throws<NullReferenceException>(() => entriesController.GetAll());
        }
    }
}
