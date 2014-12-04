using Repository.Pattern.Repository;
using ShowManagement.Business.Models;
using ShowManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ShowManagement.Repository.Repositories
{
    public static class ShowRepository
    {
        #region GetShow
        public static Show GetShow(this IRepositoryAsync<Show> repository, int showId, bool includeParsers)
        {
            var query = repository.Queryable();

            if (includeParsers)
            {
                query = query.Include(s => s.ShowParsers);
            }

            var show = query.SingleOrDefault(s => s.ShowId == showId);

            return show;
        }
        #endregion

        #region GetShowInfos
        public static IList<ShowInfo> GetShowInfos(this IRepositoryAsync<Show> repository)
        {
            var query = repository.BuildShowInfoQuery();

            var showInfoQueryEntities = query.ToList();

            var showInfos = showInfoQueryEntities.Select(siqe =>
                {
                    siqe.ShowInfo.Parsers.AddRange(siqe.Parsers);

                    return siqe.ShowInfo;
                }).ToList();

            return showInfos;
        }
        public static List<ShowInfo> GetShowInfos(this IRepositoryAsync<Show> repository, List<int> showIds)
        {
            var query = repository.BuildShowInfoQuery();

            var showInfoQueryEntities = query.Where(siqe => showIds.Contains(siqe.ShowInfo.ShowId)).ToList();

            var showInfos = showInfoQueryEntities.Select(siqe =>
                {
                    siqe.ShowInfo.Parsers.AddRange(siqe.Parsers);

                    return siqe.ShowInfo;
                }).ToList();

            return showInfos;
        }
        public static List<ShowInfo> GetShowInfos(this IRepositoryAsync<Show> repository, string directoryPath)
        {
            var query = repository.BuildShowInfoQuery();

            var showInfoQueryEntities = query.Where(siqe => siqe.ShowInfo.Directory == directoryPath).ToList();

            var showInfos = showInfoQueryEntities.Select(siqe =>
            {
                siqe.ShowInfo.Parsers.AddRange(siqe.Parsers);

                return siqe.ShowInfo;
            }).ToList();

            return showInfos;
        }
        public static ShowInfo GetShowInfo(this IRepositoryAsync<Show> repository, int showId)
        {
            List<ShowInfo> showInfos = repository.GetShowInfos(new List<int> { showId });

            ShowInfo showInfo = showInfos.SingleOrDefault();

            return showInfo;
        }
        

        private static IQueryable<ShowInfoQueryEntity> BuildShowInfoQuery(this IRepositoryAsync<Show> repository)
        {
            var query = repository
                .Queryable()
                .Select(s =>
                    new ShowInfoQueryEntity
                    {
                        ShowInfo = new ShowInfo
                        {
                            ShowId = s.ShowId,
                            TvdbId = s.TvdbId,
                            ImdbId = s.ImdbId,
                            Name = s.Name,
                            Directory = s.Directory,
                        },
                        Parsers = s.ShowParsers
                            .Select(sp => new Parser
                            {
                                ParserId = sp.ShowParserId,
                                TypeKey = sp.Type,
                                Pattern = sp.Pattern,
                                ExcludedCharacters = sp.ExcludedCharacters,
                            })
                    });

            return query;
        }

        private class ShowInfoQueryEntity
        {
            public ShowInfo ShowInfo { get; set; }
            public IEnumerable<Parser> Parsers { get; set; }
        } 
        #endregion

        

        public static IEnumerable<Show> GetShowsMissingParsers(this IRepositoryAsync<Show> repository)
        {
            var query = repository
                .Queryable()
                .Where(s => !s.ShowParsers.Any());

            var result = query.AsEnumerable();

            return result;
        }
    }
}
