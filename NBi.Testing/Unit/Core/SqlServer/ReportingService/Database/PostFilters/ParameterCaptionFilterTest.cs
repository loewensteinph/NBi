using NBi.Core.SqlServer.ReportingService;
using NBi.Core.SqlServer.ReportingService.Database;
using NBi.Core.SqlServer.ReportingService.Database.Builders;
using NBi.Core.SqlServer.ReportingService.Database.PostFilters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.SqlServer.ReportingService.Database.Builders
{
    [TestFixture]
    public class ParameterCaptionFilterTest
    {
        [Test]
        public void Evaluate_ValidCaption_True()
        {
            var filter = new ParameterCaptionFilter("my-cation");
            var row = new ReportingModelRow() { Caption = "my-cation" };

            Assert.That(filter.Evaluate(row), Is.True);
        }

        [Test]
        public void Evaluate_InvalidCaption_False()
        {
            var filter = new ParameterCaptionFilter("my-cation");
            var row = new ReportingModelRow() { Caption = "not-my-cation" };

            Assert.That(filter.Evaluate(row), Is.False);
        }
    }
}
