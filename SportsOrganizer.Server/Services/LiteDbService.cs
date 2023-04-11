using LiteDB;
using SportsOrganizer.Server.Interfaces;

namespace SportsOrganizer.Server.Services;

public class LiteDbService<T> : ILiteDbService<T>
{
    private readonly IConfiguration _configuration;
    private readonly string _collectionName;

    public LiteDbService(IConfiguration configuration)
    {
        _configuration = configuration;
        _collectionName = typeof(T).Name;
    }

    public T Get(int id)
    {
        using (var db = new LiteDatabase(_configuration["LiteDb:DatabaseName"]))
        {
            var collection = db.GetCollection<T>(_collectionName);
            return collection.FindById(id);
        }
    }

    public IEnumerable<T> GetAll()
    {
        using (var db = new LiteDatabase(_configuration["LiteDb:DatabaseName"]))
        {
            var collection = db.GetCollection<T>(_collectionName);
            return collection.FindAll().ToList();
        }
    }

    public int Insert(T entity)
    {
        using (var db = new LiteDatabase(_configuration["LiteDb:DatabaseName"]))
        {
            var collection = db.GetCollection<T>(_collectionName);
            return collection.Insert(entity);
        }
    }

    public bool Update(T entity)
    {
        using (var db = new LiteDatabase(_configuration["LiteDb:DatabaseName"]))
        {
            var collection = db.GetCollection<T>(_collectionName);
            return collection.Update(entity);
        }
    }

    public bool Delete(int id)
    {
        using (var db = new LiteDatabase(_configuration["LiteDb:DatabaseName"]))
        {
            var collection = db.GetCollection<T>(_collectionName);
            return collection.Delete(id);
        }
    }
}
