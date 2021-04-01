using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XlsxCompare;

namespace XlsxCompare.Tests
{
    [TestClass]
    public class DependencyTests
    {

        [TestMethod]
        public void EPPlus_Version_IsLowEnough()
        {
            var dep = typeof(Program).Assembly.GetReferencedAssemblies()
                .Single(x => x.Name == "EPPlus");

            Assert.AreEqual(4, dep.Version.Major, "v4 is the last openly licensed version");
        }
    }
}
