using backend.Models;
using FaceAiSharp;
using MongoDB.Driver;

namespace backend.Data.Repositories;

public class FacesRepository : IFacesRepository
{
    private IMongoCollection<Face> collection;
    
    public FacesRepository(DbContext context)
    {
        collection = context.Faces;
    }

    public async Task Insert(Face face)
    {
        await collection.InsertOneAsync(face);
    }

    public async Task<List<Face>> GetAll()
    {
        var faces = await collection.FindAsync(_ => true);
        return await faces.ToListAsync();
    }
}