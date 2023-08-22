
using CompagnyTools.Context;
using CompagnyTools.Entities;
using CompagnyTools.GenericRepository;


namespace CompagnyTools.UnitOfWork
{
    public class CompagnyToolsUnitOfWork : IDisposable
    {
        #region Private properties
        private readonly EFCoreContext _context;
        private bool disposed;
        private GenericRepository<User> _userRepository;

        #endregion

        public CompagnyToolsUnitOfWork(EFCoreContext context)
        {
            _context = context;
        }


        #region Repository public methods

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new GenericRepository<User>(_context);
                }
                return _userRepository;
            }
        }
        #endregion

        #region Public methods
        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();

        }

        public EFCoreContext GetContext()
        {
            return this._context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
