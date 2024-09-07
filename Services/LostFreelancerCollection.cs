using MongoDB.Bson;
using MongoDB.Driver;
using StarportBot.Common;
using StarportBot.Models;

namespace StarportBot.Services;

public class LostFreelancerCollection
{
    private readonly IMongoCollection<LostUser> _lostUsers;
    
    public LostFreelancerCollection(ApplicationConfiguration configuration)
    {
        var client = new MongoClient(configuration.DatabaseSettings.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseSettings.Database);
        _lostUsers = database.GetCollection<LostUser>(configuration.DatabaseSettings.LostUserCollection);
    }
    
    public async Task<LostUser?> GetLastLostFreelancer()
    {
        var filter = Builders<LostUser>.Filter.Empty;
        var sort = Builders<LostUser>.Sort.Descending(x => x.Time);
        var user = await _lostUsers.FindAsync(filter, new FindOptions<LostUser, LostUser>()
        {
            Sort = sort
        });

        return await user.FirstOrDefaultAsync();
    }

    public async Task<List<(ulong, int)>> GetAdminLeaderboard()
    {
        var aggregate = await _lostUsers
            .Aggregate()
            .Group(x => x.BanningAdmin, x => new
            {
                _id = x.Key,
                Count = x.Count()
            })
            .ToListAsync();

        return aggregate.Select(x => (x._id, x.Count)).ToList();
    }
    
    public async Task<bool> AddNewLostFreelancerAsync(LostUser user)
    {
        if (user.BanningAdmin is 0 || user.BannedUser is 0)
        {
            return false;
        }
        
        user.Time = DateTime.UtcNow;
        user.Id = ObjectId.GenerateNewId();

        await _lostUsers.InsertOneAsync(user);

        return true;
    }
}