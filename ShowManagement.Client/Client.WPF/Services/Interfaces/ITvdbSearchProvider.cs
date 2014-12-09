using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    public interface ITvdbSearchProvider
    {
        Task<List<SeriesSearchResult>> SearchForSeries(string seriesTitle);
    }
}