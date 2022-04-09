using Dapper;
using TodoServerSide.Models;
using TodoServerSide.Utilities;

namespace TodoServerSide.Repositories;


public interface IUserRepository
{
    Task<User> Create(User Data);
    Task<User> GetByUserName(string UserName);
    Task<User> GetById(long Id);
}
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<User> Create(User Data)
    {
        var query = $@"INSERT INTO ""{TableNames.users}"" (username,password)
        VALUES(@UserName,@Password) RETURNING *;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, Data);
    }

    public  async Task<User> GetByUserName(string UserName)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}"" WHERE username = @UserName;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { UserName });
    }

    public async Task<User> GetById(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.users}"" WHERE id = @Id;";
        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Id });
    }
}