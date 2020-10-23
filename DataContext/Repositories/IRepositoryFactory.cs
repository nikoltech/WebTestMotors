namespace WebTestMotors.DataAccess.Repositories
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>(RepositoryItemOptions options);
    }
}
