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
using NBi.Core.Report;

namespace NBi.Testing.Unit.Core.SqlServer.ReportingService.Database.Builders
{
    [TestFixture]
    public class ParameterDiscoveryCommandBuilderTest
    {
        [Test]
        public void GetCommandText_PathReportFilters_CorrectStatement()
        {
            var filters = new CaptionFilter[]
            {
                new CaptionFilter(Target.Path, "/path/sub-path")
                , new CaptionFilter(Target.Report, "my-report")
                , new CaptionFilter(Target.Parameter, "parameter-caption")
            };

            var builder = new ParameterDiscoveryCommandBuilder();
            builder.Build(filters);
            var commandText = builder.GetCommandText();
            Assert.That(commandText.Replace(" ","").ToLower(), Is.StringContaining("='/path/sub-path'".ToLower()));
            Assert.That(commandText.Replace(" ", "").ToLower(), Is.StringContaining("='my-report'".ToLower()));
            Assert.That(commandText.Replace(" ", "").ToLower(), Is.Not.StringContaining("[parameter]".ToLower()));
        }

        [Test]
        public void GetCommandText_CaptionFilter_CorrectPostFilter()
        {
            var filters = new CaptionFilter[]
            {
                new CaptionFilter(Target.Path, "/path/sub-path")
                , new CaptionFilter(Target.Report, "my-report")
                , new CaptionFilter(Target.Parameter, "parameter-caption")
            };

            var builder = new ParameterDiscoveryCommandBuilder();
            builder.Build(filters);
            var postFilters = builder.GetPostFilters();
            Assert.That(postFilters.Count, Is.EqualTo(1));
            Assert.That(postFilters.ElementAt(0), Is.TypeOf<ParameterCaptionFilter>());
        }

    }
}
