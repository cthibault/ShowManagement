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
    class MockServiceProvider : IServiceProvider
    {
        public MockServiceProvider()
        {
            this._shows = new List<ShowInfo>
            {
                new ShowInfo
                {
                    ShowId = 1,
                    Name = "Show 1",
                    Directory = @"C:\Shows\Show 1",
                    TvdbId = 1,
                    ImdbId = "1",
                    Parsers = new List<Parser>
                    {
                        new Parser
                        {
                            ParserId = 1,
                            Type = ParserType.Season,
                            Pattern = "sPattern",
                            ExcludedCharacters = "sx",
                        },
                        new Parser
                        {
                            ParserId = 2,
                            Type = ParserType.Episode,
                            Pattern = "ePattern",
                            ExcludedCharacters = "ex",
                        },
                    },
                },
                new ShowInfo
                {
                    ShowId = 2,
                    Name = "Show 2",
                    Directory = @"C:\Shows\Show 2",
                    TvdbId = 2,
                    ImdbId = "2",
                    Parsers = new List<Parser>
                    {
                        new Parser
                        {
                            ParserId = 3,
                            Type = ParserType.Season,
                            Pattern = "sPattern",
                            ExcludedCharacters = "sx",
                        },
                    },
                },
                new ShowInfo
                {
                    ShowId = 3,
                    Name = "Show 3",
                    Directory = @"C:\Shows\Show 3",
                    TvdbId = 3,
                    ImdbId = "3",
                    Parsers = new List<Parser>
                    {
                        new Parser
                        {
                            ParserId = 4,
                            Type = ParserType.Episode,
                            Pattern = "ePattern",
                            ExcludedCharacters = "ex",
                        },
                    },
                },
                new ShowInfo
                {
                    ShowId = 4,
                    Name = "Show 4",
                    Directory = @"C:\Shows\Show 4",
                    TvdbId = 4,
                    ImdbId = "4",
                    Parsers = new List<Parser>(),
                },
                new ShowInfo
                {
                    ShowId = 5,
                    Name = "Show 5",
                    Directory = @"C:\Shows\Show 5",
                    TvdbId = 0,
                    ImdbId = null,
                    Parsers = new List<Parser>(),
                },
            };
        }

        public async Task<List<ShowInfo>> GetAllShows()
        {
            var results = this._shows.Select(s => this.Copy(s, true)).ToList();

            return results;
        }

        public async Task<ShowInfo> GetShow(int showId)
        {
            ShowInfo result = null;

            var foundShow = this._shows.SingleOrDefault(s => s.ShowId == showId);
            if (foundShow != null)
            {
                result = this.Copy(foundShow, true);
            }

            return result;
        }

        public async Task<ShowInfo> SaveShow(ShowInfo showInfo)
        {
            ShowInfo result = null;

            switch (showInfo.ObjectState)
            {
                case ObjectState.Added:
                    var showCopy = this.Copy(showInfo, false);
                    showCopy.ShowId = this._shows.Max(s => s.ShowId) + 1;

                    foreach (var parser in showInfo.Parsers)
                    {
                        await this.SaveParser(showCopy, parser);
                    }

                    this._shows.Add(showCopy);

                    result = this.Copy(showCopy, true);
                    break;
                case ObjectState.Modified:
                    var showToUpdate = this._shows.SingleOrDefault(s => s.ShowId == showInfo.ShowId);
                    if (showToUpdate != null)
                    {
                        showToUpdate.Name = showInfo.Name;
                        showToUpdate.Directory = showInfo.Directory;
                        showToUpdate.TvdbId = showInfo.TvdbId;
                        showToUpdate.ImdbId = showInfo.ImdbId;

                        foreach (var parser in showInfo.Parsers)
                        {
                            await this.SaveParser(showToUpdate, parser);
                        }

                        result = this.Copy(showToUpdate, true);
                    }
                    else
                    {
                        throw new InvalidOperationException("showInfo is marked as MODIFIED, but no record exists in the DB");
                    }
                    break;
                case ObjectState.Deleted:
                    var showToDelete = this._shows.SingleOrDefault(s => s.ShowId == showInfo.ShowId);
                    if (showToDelete != null)
                    {
                        this._shows.Remove(showToDelete);
                    }
                    break;
                case ObjectState.Unchanged:
                    var unchangedShow = this._shows.SingleOrDefault(s => s.ShowId == showInfo.ShowId);
                    
                    foreach (var parser in showInfo.Parsers)
                    {
                        await this.SaveParser(unchangedShow, parser);
                    }

                    result = this.Copy(unchangedShow, true);
                    break;
            }

            return result;
        }

        private async Task SaveParser(ShowInfo showInfo, Parser parser)
        {
            if (showInfo != null)
            {
                switch (parser.ObjectState)
                {
                    case ObjectState.Added:
                        var parserCopy = this.Copy(parser);
                        parserCopy.ParserId = this._shows.SelectMany(s => s.Parsers).Max(p => p.ParserId) + 1;

                        showInfo.Parsers.Add(parserCopy);
                        break;
                    case ObjectState.Modified:
                        var parserToUpdate = showInfo.Parsers.SingleOrDefault(p => p.ParserId == parser.ParserId);
                        if (parserToUpdate != null)
                        {
                            parserToUpdate.TypeKey = parser.TypeKey;
                            parserToUpdate.Pattern = parser.Pattern;
                            parserToUpdate.ExcludedCharacters = parser.ExcludedCharacters;
                        }
                        else
                        {
                            parser.ObjectState = ObjectState.Added;
                            this.SaveParser(showInfo, parser);
                        }
                        break;
                    case ObjectState.Deleted:
                        showInfo.Parsers.RemoveAll(p => p.ParserId == parser.ParserId);
                        break;
                    case ObjectState.Unchanged:

                        break;
                }
            }
        }

        public async Task<List<ShowInfo>> SaveShows(List<ShowInfo> showInfos)
        {
            List<ShowInfo> results = new List<ShowInfo>(showInfos.Count);

            foreach (var showInfo in showInfos)
            {
                var result = await this.SaveShow(showInfo);

                if (result != null)
                {
                    results.Add(result);
                }
            }

            await Task.Delay(2000);

            return results;
        }

        public readonly List<ShowInfo> _shows;


        private ShowInfo Copy(ShowInfo original, bool includeNavProperties)
        {
            var result = new ShowInfo
            {
                ShowId = original.ShowId,
                Name = original.Name,
                Directory = original.Directory,
                TvdbId = original.TvdbId,
                ImdbId = original.ImdbId
            };

            if (includeNavProperties)
            {
                result.Parsers = new List<Parser>(original.Parsers.Select(p => this.Copy(p)));
            }

            return result;
        }
        private Parser Copy(Parser original)
        {
            var result = new Parser
            {
                ParserId = original.ParserId,
                TypeKey = original.TypeKey,
                Pattern = original.Pattern,
                ExcludedCharacters = original.ExcludedCharacters
            };

            return result;
        }
    }
}
