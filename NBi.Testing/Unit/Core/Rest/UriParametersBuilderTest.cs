using NBi.Core.Rest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Rest
{
    [TestFixture()]
    public class UriParametersBuilderTest
    {
        [Test]
        public void Build_Parameters_CorrectUri()
        {
            var builder = new UriParametersBuilder();
            var parameters = new Dictionary<string, string>();
            parameters.Add("calendar", "default");
            parameters.Add("year", "2015");
            parameters.Add("month", "6");
            parameters.Add("day", "25");
            builder.Setup("calendars/$calendar$/$year$/$month$/$day$", parameters);
            builder.Build();

            Assert.That(builder.GetUri(), Is.EqualTo("calendars/default/2015/6/25"));
        }

        [Test]
        public void Build_Parameters_CorrectRemainingParameters()
        {
            var builder = new UriParametersBuilder();
            var parameters = new Dictionary<string, string>();
            parameters.Add("calendar", "default");
            parameters.Add("year", "2015");
            parameters.Add("month", "6");
            parameters.Add("day", "25");
            builder.Setup("calendars/$calendar$/$year$/$month$/$day$", parameters);
            builder.Build();

            Assert.That(builder.GetRemainingParameters(), Is.Empty);
        }

        [Test]
        public void Build_MissingParameters_Exception()
        {
            var builder = new UriParametersBuilder();
            var parameters = new Dictionary<string, string>();
            parameters.Add("calendar", "default");
            parameters.Add("year", "2015");
            parameters.Add("month", "6");
            builder.Setup("calendars/$calendar$/$year$/$month$/$day$", parameters);

            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Test]
        public void Build_AdditionalParameters_CorrectRemainingParameters()
        {
            var builder = new UriParametersBuilder();
            var parameters = new Dictionary<string, string>();
            parameters.Add("calendar", "default");
            parameters.Add("year", "2015");
            parameters.Add("month", "6");
            parameters.Add("day", "25");
            parameters.Add("limit", "100");
            builder.Setup("calendars/$calendar$/$year$/$month$/$day$", parameters);
            builder.Build();

            Assert.That(builder.GetRemainingParameters(), Has.Count.EqualTo(1));
            Assert.That(builder.GetRemainingParameters().Keys, Has.Member("limit"));
            Assert.That(builder.GetRemainingParameters()["limit"], Is.EqualTo("100"));
        }
    }
}
