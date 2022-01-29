using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoDomain.Sql.Context;
using ToDoTest.Common;
using ToDoWebApp.Api;
using ToDoWebApp.Dtos;
using Xunit;

namespace ToDoTest
{
    public class ToDoTest
    {

        [Fact]
        public async Task ToDoListTest()
        {

            var db = new ToDoDb().CreateContext();

            var listController = new ToDoListController(db);
            var entryController = new ToDoEntryController(db);


            var lists = new ToDoListDto[10];

            // list create
            for(int i = 0; i< 10; i++)
            {
                lists[i] = (await listController.Save(GetListDto($"list {i}"))).Value;
            }

            var addesLists = (await listController.GetAll()).Value;

            Assert.Equal(10, addesLists.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.Contains(lists[i].Name, addesLists.Select(l => l.Name));
                Assert.Contains(lists[i].Id, addesLists.Select(l => l.Id));
                Assert.True(lists[i].IsCompleted);
                Assert.Equal(0, lists[i].NumberOfEntries);
            }

            // Update

            lists[2].Name = $"{lists[2].Name} upt";
            lists[5].Name = $"{lists[5].Name} upt";
            lists[7].Name = $"{lists[7].Name} upt";


            lists[2] = (await listController.Update(lists[2].Id, lists[2])).Value;
            lists[5] = (await listController.Update(lists[5].Id, lists[5])).Value;
            lists[7] = (await listController.Update(lists[7].Id, lists[7])).Value;


            addesLists = (await listController.GetAll()).Value;

            for (int i = 0; i < 10; i++)
            {
                Assert.Contains(lists[i].Name, addesLists.Select(l => l.Name));
                Assert.Contains(lists[i].Id, addesLists.Select(l => l.Id));
                Assert.True(lists[i].IsCompleted);
                Assert.Equal(0, lists[i].NumberOfEntries);
            }


            // Delete

            await listController.Delete(lists[0].Id);
            addesLists = (await listController.GetAll()).Value;

            Assert.DoesNotContain(lists[0].Id, addesLists.Select(l => l.Id));

        }



        [Fact]
        public async Task ToDoListEntries()
        {

            var db = new ToDoDb().CreateContext();

            var listController = new ToDoListController(db);
            var entryController = new ToDoEntryController(db);


            var lists = new ToDoListDto[10];

            for (int i = 0; i < 10; i++)
            {
                lists[i] = (await listController.Save(GetListDto($"list {i}"))).Value;
            }

            for (int i = 0; i < 10; i++)
            {
                var entries = (await entryController.GetByListId(lists[i].Id)).Value;
                Assert.Empty(entries);
            }


            var entryList0 = new ToDoEntryDto[7];
            var entryList1 = new ToDoEntryDto[7];

            for (int i = 0; i < 7; i++)
            {
                entryList0[i] = (await entryController.Save(GetEntryDto(lists[0].Id, $"entry0 {i}"))).Value;
                entryList1[i] = (await entryController.Save(GetEntryDto(lists[1].Id, $"entry1 {i}"))).Value;

            }

            var list0 = (await listController.Get(lists[0].Id)).Value;

            Assert.False(list0.IsCompleted);
            Assert.Equal(7, list0.NumberOfEntries);

            var addedEntry0 = (await entryController.GetByListId(lists[0].Id)).Value;

            Assert.Equal(7, addedEntry0.Count);

            for (int i = 0; i < 7; i++)
            {
                Assert.Contains(entryList0[i].Content, addedEntry0.Select(e => e.Content));
                Assert.Contains(entryList0[i].ExpirationDate, addedEntry0.Select(e => e.ExpirationDate));
            }


            // Update

            var now = DateTime.UtcNow;

            entryList1[2].ExpirationDate = now.AddDays(-2);
            entryList1[5].ExpirationDate = now.AddDays(5);
            entryList1[6].ExpirationDate = now.AddDays(6);

            entryList1[2] = (await entryController.Update(entryList1[2].Id, entryList1[2])).Value;

            entryList1[5] = (await entryController.Update(entryList1[5].Id, entryList1[5])).Value;
            entryList1[6] = (await entryController.Update(entryList1[6].Id, entryList1[6])).Value;


            var addedEntry1 = (await entryController.GetByListId(lists[1].Id)).Value;

            Assert.Equal(7, addedEntry0.Count);

            for (int i = 0; i < 7; i++)
            {
                Assert.Contains(entryList1[i].Content, addedEntry1.Select(e => e.Content));
                Assert.Contains(entryList1[i].ExpirationDate, addedEntry1.Select(e => e.ExpirationDate));
            }

            Assert.Single(addedEntry1.Where(e => e.IsExpired));



            // complete
            for (int i = 0; i < 7; i++)
            {
                await entryController.SetDone(addedEntry1[i].Id);
            }


            addedEntry1 = (await entryController.GetByListId(lists[1].Id)).Value;

            lists[1] = (await listController.Get(lists[1].Id)).Value;


            for (int i = 0; i < 7; i++)
            {
                Assert.True(addedEntry1[i].IsDone);
            }

            Assert.True(lists[1].IsCompleted);


            // Delete
            await entryController.Delete(entryList0[0].Id);

            addedEntry0 = (await entryController.GetByListId(lists[0].Id)).Value;

            Assert.DoesNotContain(entryList0[0].Id, addedEntry0.Select(l => l.Id));
            lists[0] = (await listController.Get(lists[0].Id)).Value;

            Assert.Equal(6, lists[0].NumberOfEntries);

        }

        [Fact]
        public async Task ErrorTest()
        {

            var db = new ToDoDb().CreateContext();

            var listController = new ToDoListController(db);
            var entryController = new ToDoEntryController(db);

            var response = (await listController.Get("not_exist_iddex")).Result as ObjectResult;
            Assert.Equal(404, response.StatusCode);

            response = (await listController.Update("not_exist_iddex", GetListDto("asd"))).Result as ObjectResult;
            Assert.Equal(404, response.StatusCode);

            var response2 = (await listController.Delete("not_exist_iddex")) as ObjectResult;
            Assert.Equal(404, response.StatusCode);


            response = (await entryController.Get("not_exist_iddex")).Result as ObjectResult;
            Assert.Equal(404, response.StatusCode);

            response = (await entryController.Save(GetEntryDto("not_existing_index", "asd") )).Result as ObjectResult;
            Assert.Equal(404, response.StatusCode);

            response = (await entryController.Update("not_existing_index",GetEntryDto("pippo", "asd"))).Result as ObjectResult;
            Assert.Equal(404, response.StatusCode);

            response = (await entryController.Delete("not_exist_iddex")) as ObjectResult;
            Assert.Equal(404, response.StatusCode);


        }

        private ToDoListDto GetListDto(string name)
        {
            return new ToDoListDto()
            {
                Name = name,
            };
        }

        private ToDoEntryDto GetEntryDto(string listId, string content, DateTime? expireDate = null)
        {
            return new ToDoEntryDto()
            {
                ToDoListId = listId,
                Content = content,
                ExpirationDate = expireDate,
            };
        }
    }
}
