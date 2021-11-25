using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Controllers;
using my_diary.Api.Model;
using System;
using System.Collections;
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
            var result = entriesController.CreateEntry(entry);
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
        public void CreateEntry_InvalidEntry_ReturnsBadRequest()
        {
            var entriesController = new EntriesController(db);
            var result = entriesController.CreateEntry(new Entry());
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAll_NoEntriesExist_ReturnsErrorCode404()
        {
            var entriesController = new EntriesController(db);
            var response = entriesController.GetAll();
            Assert.IsAssignableFrom<NotFoundObjectResult>(response); 
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

            var result = entriesController.GetAll();
            var okObjResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var entries = okObjResult.Value;
            Assert.IsType<List<Entry>>(entries);
            Assert.NotEmpty((IEnumerable)entries);

        }

        [Fact]
        public void GetOne_UseWrongId_ReturnsBadRequest()
        {
            var entriesController = new EntriesController(db);
            var result = entriesController.GetOne("test");
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
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
            var returned = Assert.IsAssignableFrom<OkObjectResult>(entry);
            var item = Assert.IsType<Entry>(returned.Value);
            Assert.Equal(testEntry.Id, item.Id);
        }

        [Fact]
        public void UpdateEntry_PassInvalidParams_ReturnsBadRequest()
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

            var result = entriesController.UpdateEntry(testEntry.Id, testEntry);
            Assert.IsAssignableFrom<OkObjectResult>(result);

        }
    }
}
