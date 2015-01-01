using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Client.WPF.Services;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class RecentDownloadViewModel : BaseViewModel
    {
        public RecentDownloadViewModel(ShowDownloadInfo showDownloadInfo)
            : base()
        {
            if (showDownloadInfo == null)
            {
                throw new ArgumentNullException("showDownloadInfo");
            }

            this.Model = showDownloadInfo;
        }

        public ShowDownloadInfo Model
        {
            get { return this._model; }
            private set 
            { 
                this.RaiseAndSetIfChanged(ref this._model, value);

                this._title = null;
                this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Title));
            }
        }
        private ShowDownloadInfo _model;

        public string Title
        {
            get
            {
                if (this.Model != null && this._title == null)
                {
                    string[] pieces = this.Model.CurrentPath.Split(new[] { Path.DirectorySeparatorChar });

                    if (pieces.Count() >= 3)
                    {
                        pieces = pieces.Skip(pieces.Count() - 3).ToArray();
                    }

                    this._title = string.Join(Path.DirectorySeparatorChar.ToString(), pieces);
                }

                return this._title;
            }
        }
        private string _title = null;
    }
}
