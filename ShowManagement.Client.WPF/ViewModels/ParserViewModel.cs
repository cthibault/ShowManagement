using Entities.Pattern;
using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Enums;
using ShowManagement.Business.Models;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class ParserViewModel : TrackableObject
    {
        public ParserViewModel(IUnityContainer unityContainer, Parser parser)
            : base(unityContainer, true)
        {
            this._model = parser;

            this.InitializeData();
            this.InitializeCommands();
        }

        private void InitializeData()
        {
            this.ParserTypes = new List<ItemViewModel<ParserType, int>>
            {
                new ItemViewModel<ParserType, int>(ParserType.Episode, (int)ParserType.Episode, ParserType.Episode.ToString()),
                new ItemViewModel<ParserType, int>(ParserType.Season, (int)ParserType.Season, ParserType.Season.ToString()),
            };

            this.HasChangesObservable.Subscribe(hasChanges =>
            {
                if (this.ObjectState != ObjectState.Added && this.ObjectState != ObjectState.Deleted)
                {
                    this.ObjectState = hasChanges ? ObjectState.Modified : ObjectState.Unchanged;
                }
            });
        }
        private void InitializeCommands()
        {

        }

        #region Wrapper Properties
        public int ParserId
        {
            get { return this._model.ParserId; }
        }

        public int TypeKey
        {
            get { return this._model.TypeKey; }
            set { this.LogRaiseAndSetIfChanged(this._model.TypeKey, value, v => this._model.TypeKey = v); }
        }

        public string Pattern
        {
            get { return this._model.Pattern; }
            set { this.LogRaiseAndSetIfChanged(this._model.Pattern, value, v => this._model.Pattern = v); }
        }

        public string ExcludedCharacters
        {
            get { return this._model.ExcludedCharacters; }
            set { this.LogRaiseAndSetIfChanged(this._model.ExcludedCharacters, value, v => this._model.ExcludedCharacters = v); }
        }

        public ObjectState ObjectState
        {
            get { return this._model.ObjectState; }
            set
            {
                if (this._model.ObjectState != value)
                {
                    this.RaisePropertyChanging(this.ExtractPropertyName(x => x.ObjectState));
                    this._model.ObjectState = value;
                    this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ObjectState));
                }
            }
        }

        #endregion

        public bool IsNew
        {
            get { return this.ParserId == 0; }
        }

        public List<ItemViewModel<ParserType, int>> ParserTypes { get; private set; }


        public Parser Model
        {
            get { return this._model; }
        }
        private Parser _model;
    }
}
