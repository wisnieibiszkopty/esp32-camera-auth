using backend.Models;

namespace backend.Data.Repositories;

public interface IFaceRepository
{
    public Task SaveAsync(FaceData face);
    public Task<List<FaceData>> GetAll();
    public Task<FaceData> GetById(string id);
    public Task<FaceData?> GetByPerson(string person);
    public Task<bool> DeleteByPersonName(string name);
    public Task<long> GetCount();
}