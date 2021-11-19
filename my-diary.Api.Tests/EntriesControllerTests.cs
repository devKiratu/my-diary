using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Controllers;
using my_diary.Api.Model;
using System;
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
    }
}
