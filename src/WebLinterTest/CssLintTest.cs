﻿// Modifications Copyright Rich Newman 2017
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class CsslintTest
    {
        [TestMethod, TestCategory("CssLint")]
        public async Task Standard()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/csslint/a.css");
            Assert.IsTrue(result.Length == 0);
            //var first = result.First();
            //Assert.IsTrue(first.HasErrors);
            //Assert.IsFalse(result.First().Errors.First().IsError, result.First().Errors.First().ErrorCode + " is not 'warning'");
            //Assert.IsFalse(string.IsNullOrEmpty(first.Errors.First().FileName));
            //Assert.AreEqual(1, first.Errors.Count);
        }

        [TestMethod, TestCategory("CssLint")]
        public async Task Multiple()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/csslint/a.css", "../../artifacts/csslint/b.css");
            Assert.IsTrue(result.Length == 0);
            //Assert.IsTrue(result.First().HasErrors);
            //Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName));
            //Assert.AreEqual(3, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("CssLint")]
        public async Task FileNotExist()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/csslint/doesntexist.css");
            //Assert.IsTrue(result.First().HasErrors);
            // Running on css file should have same result as any other non-TS file
            Assert.IsTrue(result.Length == 0);

        }
    }
}
