using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ITestRepository : IRepository 
    {
        Task<bool> Test();
    
    }


    public class TestRepository: ITestRepository
    {
        private readonly MainDBContext _dbContext;
        public TestRepository(MainDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Test()
        {
            _dbContext.Posts.Add(new Post { BlogId=1, Content="test", PostId=1, Title="test"   });
            _dbContext.SaveChanges();
            return  await _dbContext.Database.CanConnectAsync();
        }
    }
}
