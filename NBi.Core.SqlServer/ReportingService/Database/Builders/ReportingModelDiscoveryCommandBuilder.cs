using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using NBi.Core.SqlServer.ReportingService;
using NBi.Core.Report;
using NBi.Core.Model;

namespace NBi.Core.SqlServer.ReportingService.Database.Builders
{
    abstract class ReportingModelDiscoveryCommandBuilder : IDiscoveryCommandBuilder
    {
        private string commandText;
        private IEnumerable<IPostCommandFilter> postFilters;
        private bool isBuild = false;

        public void Build(IEnumerable<IFilter> filters)
        {
            commandText = BuildCommandText();

            var captionFilters = BuildCaptionFilters(filters.Where(f => f is CaptionFilter).Cast<CaptionFilter>());
            var otherFilters = BuildNonCaptionFilters(filters.Where(f => !(f is CaptionFilter)));

            var allFilters = captionFilters.Union(otherFilters).ToList();
            var commandFilters = allFilters.Where(f => f is CommandFilter).Cast<CommandFilter>();
            var valueFilters = commandFilters.Select(f => f.Value);

            var filterQueryToken = string.Empty;
            foreach (var valueFilter in valueFilters)
                filterQueryToken += " and " + valueFilter;

            commandText = commandText.Replace("--{Filters}--", filterQueryToken);

            postFilters = allFilters.Where(f => f is IPostCommandFilter).Cast<IPostCommandFilter>();
            isBuild = true;
        }



        protected abstract IEnumerable<ICommandFilter> BuildCaptionFilters(IEnumerable<CaptionFilter> filters);
        protected virtual IEnumerable<IFilter> BuildNonCaptionFilters(IEnumerable<IFilter> filters)
        {
            return new List<ICommandFilter>();
        }

        protected abstract string BuildCommandText();

        public string GetCommandText()
        {
            if (!isBuild)
                throw new InvalidOperationException();

            return commandText;
        }

        public IEnumerable<IPostCommandFilter> GetPostFilters()
        {
            if (!isBuild)
                throw new InvalidOperationException();

            return postFilters;
        }

        protected string ReadQueryFromResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            
            using (Stream stream = assembly.GetManifestResourceStream(string.Format("{0}.ReportingService.Database.Resources.{1}.sql", assemblyName, fileName)))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }

        }

    }
}
