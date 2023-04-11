namespace SportsOrganizer.Server.Interfaces;

public interface ILiteDbService<T>
{
    T Get(int id);
    IEnumerable<T> GetAll();
    int Insert(T entity);
    bool Update(T entity);
    bool Delete(int id);
}
