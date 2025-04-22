using backend.Models;
using FaceAiSharp;

namespace backend.Data.Repositories;

public interface IFacesRepository
{
    public Task Insert(Face face);
    public Task<List<Face>> GetAll();
}