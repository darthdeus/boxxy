using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boxxy.Core.Tests
{
    [TestClass]
    public class ObservableCollectionTest
    {
        [TestMethod]
        public void DerpTest()
        {
            var ints = new ObservableCollection<int>();
            var strings = ints.Map(x => x.ToString());
        }
    }
}
