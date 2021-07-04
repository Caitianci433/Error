using Infrastructure.DB;
using Infrastructure.Entity;
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
        Task<Image> Test();
    
    }


    public class TestRepository: ITestRepository
    {
        private readonly MainDBContext _dbContext;
        public TestRepository(MainDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Image> Test()
        {
            var image = _dbContext.Image.Add(new Image { contentType="jpg", path= "2b8e7783c6ebd218.jpg" });
            await _dbContext.SaveChangesAsync();

            var ret = await _dbContext.Image.FirstOrDefaultAsync();

            return ret;
        }
    }
}
