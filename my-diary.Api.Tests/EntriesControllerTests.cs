using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Controllers;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace my_diary.Api.Tests
{
    public class EntriesControllerTests
    {
        private IInMemoryDb db = new InMemDb();
        [Fact]
        public void CreateEntry_AddEntry_ReturnsAddedEntry()
        {
            //Arrange
            var entriesController = new EntriesController(db);
            var entry = new Entry 
            { 
                Id = "one", 
                LastUpdated = DateTimeOffset.UtcNow, 
                Title = "Note one", 
                Text = "First note yea" 
            };
            //Act
            var result = entriesController.CreateEntry(entry).Result;
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateEntry_AddEntry_IncreasesEntriesInDbByOne()
        {
            //Arrange
            var entriesController = new EntriesController(db);
            var entry = new Entry
            {
                Id = "one",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note one",
                Text = "First note yea"
            };
            //Act
            var entriesBeforeAdd = db.Entries.Count;
            entriesController.CreateEntry(entry);
            var entriesAfterAdd = db.Entries.Count;
            var diff = entriesAfterAdd - entriesBeforeAdd;

            //Assert 
            Assert.Equal(1, diff);
            Assert.Contains(entry, db.Entries);
        }

        [Fact]
        public void GetAll_NoEntriesExist_ReturnsErrorCode404()
        {
            var entriesController = new EntriesController(db);
            var response = entriesController.GetAll();
            Assert.IsAssignableFrom<NotFoundObjectResult>(response.Result); 
        }

        [Fact]
        public void GetAll_EntriesExist_ReturnsOkResult()
        {
            var entry = new Entry
            {
                Id = "one",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note one",
                Text = "First note yea"
            };
            var inMemDb = new InMemDb();
            inMemDb.Entries.Add(entry);
            var entriesController = new EntriesController(inMemDb);

            var entries = entriesController.GetAll();
            Assert.IsAssignableFrom<OkObjectResult>(entries.Result);
            Assert.IsType<ActionResult<List<Entry>>>(entries);
        }

        [Fact]
        public void GetOne_UseWrongId_ReturnsBadRequest()
        {
            var entriesController = new EntriesController(db);
            var result = entriesController.GetOne("test");
            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetOne_ProvideValidId_ReturnEntry()
        {
            var testEntry = new Entry
            {
                Id = "one",
                LastUpdated = DateTimeOffset.UtcNow,
                Title = "Note one",
                Text = "First note yea"
            };
            var inMemDb = new InMemDb();
            inMemDb.Entries.Add(testEntry);
            var entriesController = new EntriesController(inMemDb);
            var entry = entriesController.GetOne("one");

            Assert.IsAssignableFrom<OkObjectResult>(entry.Result);
            Assert.IsType<ActionResult<Entry>>(entry);
            //TODO: Assert that testEntry == entry.Value
        }
    }
}
