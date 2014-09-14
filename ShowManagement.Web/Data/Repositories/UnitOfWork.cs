using ShowManagement.Web.Data.Entities;
using ShowManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShowManagement.Web.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        BaseRepository<Show> ShowRepository { get; }

        void Save();
    }

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Context
        private ShowManagementDbContext _context = null;
        protected ShowManagementDbContext Context
        {
            get
            {
                if (this._context == null)
                {
                    this._context = ShowManagementDbContext.Create();
                }
                return _context;
            }
            set
            {
                this._context = value;
            }
        }
        #endregion

        public BaseRepository<Show> ShowRepository
        {
            get
            {
                if (this._showRepository == null)
                {
                    this._showRepository = new BaseRepository<Show>(this.Context);
                }
                return this._showRepository;
            }
        }
        private BaseRepository<Show> _showRepository;

        public void Save()
        {
            this.Context.SaveChanges();
        }


        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this._context != null)
                    {
                        this._context.Dispose();
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}