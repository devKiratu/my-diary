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
            context.Entries.AddRange(new Entry[]
            {
               new Entry{Id = "one", LastUpdated = DateTimeOffset.UtcNow, Title = "Note one", Text = "This is the first note."},
               new Entry{Id = "two", LastUpdated = DateTimeOffset.UtcNow, Title = "Note two", Text = "This is the second note"},
               new Entry{Id = "three", LastUpdated = DateTimeOffset.UtcNow, Title = "Note five", Text = "This is the third note"},
               new Entry{Id = "four", LastUpdated = DateTimeOffset.UtcNow, Title = "Note five", Text = "This is the fourth note"},
            });
            context.SaveChanges();
        }
        public void Dispose() { }
    }

    public class EntriesControllerTests : IClassFixture<SetupFixture>
    {
        private readonly DbContextOptions<MainDbContext> _options;
        public EntriesControllerTests(SetupFixture fixture)
        {
            _options = fixture.options;
        }

        [Fact]
        public void CreateEntry_AddEntry_ReturnsAddedEntry()
        {

            using var context = new MainDbContext(_options);

            ////Arrange
            var entriesController = new EntriesController(context);
            var entry = new Entry
            {
                Id = "five",
                LastUpdated = DateTimeOffset.UtcNow,
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
            var entriesController = new EntriesController(context);
            var entry = new Entry
            {
                Id = "six",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note six",
                Text = "This is the sixth note"
            };
            //Act
            var entriesBeforeAdd = context.Entries.ToList().Count;
            entriesController.CreateEntry(entry);
            var entriesAfterAdd = context.Entries.ToList().Count;
            var diff = entriesAfterAdd - entriesBeforeAdd;

            //Assert 
            Assert.Equal(1, diff);
            Assert.Contains(entry, context.Entries);
        }

        [Fact]
        public void CreateEntry_InvalidEntry_ReturnsBadRequest()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context);
            var result = entriesController.CreateEntry(new Entry());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAll_NoEntriesExist_ReturnsErrorCode404()
        {
            using var context = new MainDbContext(new DbContextOptionsBuilder<MainDbContext>().UseInMemoryDatabase("diaryDbEmpty").Options);

            var entriesController = new EntriesController(context);
            var response = entriesController.GetAll();
            Assert.IsAssignableFrom<NotFoundObjectResult>(response);
        }

        [Fact]
        public void GetAll_EntriesExist_ReturnsOkResult()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context);

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
            var entriesController = new EntriesController(context);
            var result = entriesController.GetOne("test");
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetOne_ProvideValidId_ReturnEntry()
        {
            using var context = new MainDbContext(_options);
            var entriesController = new EntriesController(context);

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
                Id = "one",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note one update",
                Text = "Updated the first note"
            };

            var entriesController = new EntriesController(context);
            //Invalid Id, valid object
            var result = entriesController.UpdateEntry(null, testEntry);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            //Valid Id, invalid object
            var result2 = entriesController.UpdateEntry(testEntry.Id, new Entry());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result2);


        }

        [Fact]
        public void UpdateEntry_PassValidParams_ReturnsOk()
        {
            using var context = new MainDbContext(_options);

            var testEntry = new Entry
            {
                Id = "two",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note two update",
                Text = "Updated the second note"
            };

            var entriesController = new EntriesController(context);

            var result = entriesController.UpdateEntry(testEntry.Id, testEntry);
            Assert.IsAssignableFrom<OkObjectResult>(result);

        }
    }
}
