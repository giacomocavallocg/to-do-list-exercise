using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ToDoTest.Common
{
    [CollectionDefinition(nameof(ToDoDbCollectionFixture))]
    public class ToDoDbCollectionFixture : ICollectionFixture<ToDoDb>
    {
        // empty
    }
}
