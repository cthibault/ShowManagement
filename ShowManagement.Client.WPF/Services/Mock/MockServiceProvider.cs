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
            var results = this._shows.Select(this.Copy).ToList();

            return results;
        }

        public async Task<ShowInfo> GetShow(int showId)
        {
            ShowInfo result = null;

            var foundShow = this._shows.SingleOrDefault(s => s.ShowId == showId);
            if (foundShow != null)
            {
                result = this.Copy(foundShow);
            }

            return result;
        }

        public async Task<ShowInfo> SaveShow(ShowInfo showInfo)
        {
            ShowInfo result = null;

            var foundShow = this._shows.SingleOrDefault(s => s.ShowId == showInfo.ShowId);
            if (foundShow != null)
            {
                foundShow.Name = showInfo.Name;
                foundShow.Directory = showInfo.Directory;
                foundShow.TvdbId = showInfo.TvdbId;
                foundShow.ImdbId = showInfo.ImdbId;

                foreach (var parser in showInfo.Parsers)
                {
                    var foundParser = foundShow.Parsers.SingleOrDefault(p => p.ParserId == parser.ParserId);
                    if (foundShow != null)
                    {
                        foundParser.TypeKey = parser.TypeKey;
                        foundParser.Pattern = parser.Pattern;
                        foundParser.ExcludedCharacters = parser.ExcludedCharacters;
                    }
                    else
                    {
                        var parserCopy = this.Copy(parser);
                        parserCopy.ParserId = this._shows.SelectMany(s => s.Parsers).Max(p => p.ParserId) + 1;

                        foundShow.Parsers.Add(parserCopy);
                    }
                }

                result = this.Copy(foundShow);
            }
            else
            {
                var showCopy = this.Copy(showInfo);
                showCopy.ShowId = this._shows.Max(s => s.ShowId) + 1;

                this._shows.Add(showCopy);

                result = this.Copy(showCopy);
            }

            return result;
        }

        public async Task<List<ShowInfo>> SaveShows(List<ShowInfo> showInfos)
        {
            List<ShowInfo> results = new List<ShowInfo>(showInfos.Count);

            foreach (var showInfo in showInfos)
            {
                var result = await this.SaveShow(showInfo);

                results.Add(result);
            }

            await Task.Delay(5000);

            return results;
        }

        public readonly List<ShowInfo> _shows;


        private ShowInfo Copy(ShowInfo original)
        {
            var result = new ShowInfo
            {
                ShowId = original.ShowId,
                Name = original.Name,
                Directory = original.Directory,
                TvdbId = original.TvdbId,
                ImdbId = original.ImdbId
            };

            result.Parsers = new List<Parser>(original.Parsers.Select(p => this.Copy(p)));

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
