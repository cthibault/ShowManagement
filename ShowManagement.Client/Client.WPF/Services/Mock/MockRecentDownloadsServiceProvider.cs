using Entities.Pattern;
using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Services
{
    class MockRecentDownloadsServiceProvider : IRecentDownloadsServiceProvider
    {
        public MockRecentDownloadsServiceProvider()
        {
            this._showDownloadInfos = new List<ShowDownloadInfo>
            {
                new ShowDownloadInfo
                {
                    ShowDownloadId = 1,
                    CurrentPath = @"C:\Shows\Some Show\Season 1\S01.E01.Some.Show.avi",
                    OriginalPath = @"C:\Shows\Some Show\Season 1\1 - First.avi",
                    CreatedDate = DateTime.Now.AddDays(-14),
                    ModifiedDate = DateTime.Now.AddDays(-14),
                },
                new ShowDownloadInfo
                {
                    ShowDownloadId = 2,
                    CurrentPath = @"C:\Shows\Some Show\Season 1\S01.E02.Some.Show.avi",
                    OriginalPath = @"C:\Shows\Some Show\Season 1\2 - Second.avi",
                    CreatedDate = DateTime.Now.AddDays(-12),
                    ModifiedDate = DateTime.Now.AddDays(-12),
                },
                new ShowDownloadInfo
                {
                    ShowDownloadId = 3,
                    CurrentPath = @"C:\Shows\Some Other Show\Season 3\S03.E10.Some.Show.avi",
                    OriginalPath = @"C:\Shows\Some Show\Season 3\10 - Curtis.avi",
                    CreatedDate = DateTime.Now.AddDays(-7),
                    ModifiedDate = DateTime.Now.AddDays(-7),
                },
            };
        }

        public async Task<List<ShowDownloadInfo>> GetRecentDownloads()
        {
            var results = this._showDownloadInfos.Select(sdi => this.Copy(sdi)).ToList();

            return results;
        }

        public async Task<List<ShowDownloadInfo>> GetRecentDownloads(DateTime start)
        {
            var results = this._showDownloadInfos.Where(sdi => sdi.CreatedDate >= start).Select(sdi => this.Copy(sdi)).ToList();

            return results;
        }

        private ShowDownloadInfo Copy(ShowDownloadInfo original)
        {
            var result = new ShowDownloadInfo
            {
                ObjectState = ObjectState.Unchanged,
                ShowDownloadId = original.ShowDownloadId,
                OriginalPath = original.OriginalPath,
                CurrentPath = original.CurrentPath,
                CreatedDate = original.CreatedDate,
                ModifiedDate = original.ModifiedDate,
            };

            return result;
        }

        public readonly List<ShowDownloadInfo> _showDownloadInfos;
    }
}
