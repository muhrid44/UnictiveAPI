using Dapper;
using EntityModel;
using HelperClasses;
using Npgsql;
using RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HobbyRepository : IHobbyRepository
    {
        public async Task<string> AddHobby(UserModel model, string hobby)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.ExecuteAsync($@"INSERT INTO ""hobbytable""
                (""userid"", ""hobby"") VALUES
                (@Id, '{hobby}');", model);

                    return NotificationModel.Success;

                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed + ", caused by " + e;
            }
        }
        public async Task<string> DeleteHobby(UserModel model, string hobby)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.ExecuteAsync($@"DELETE FROM ""hobbytable""
                WHERE ""userid"" = @Id AND LOWER(""hobby"") = LOWER('{hobby}');", model);

                    return NotificationModel.Success;

                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed + ", caused by " + e;
            }
        }

    }
}
