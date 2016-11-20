using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.NUnit.Structure
{
    public interface IDescriptionModelHelper
    {
        string GetFilterExpression();
        string GetTargetExpression();
        string GetTargetPluralExpression();
        string GetNextTargetExpression();
        string GetNextTargetPluralExpression();
        string GetNotExpression(bool not);
    }
}
