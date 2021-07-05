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
    public interface IImageRepository : IRepository
    {
        Task<string> FetchImagePath(int id);

        Task FlushImages(IEnumerable<Image> imagePathAll);

        Task<int> FlushImageCount();
    }


    public class ImageRepository : IImageRepository
    {
        private readonly MainDBContext _dbContext;
        public ImageRepository(MainDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> FetchImagePath(int id)
        {
           return await _dbContext.Image.Where(s => s.imageId == id)
                .Select(s => s.path)
                .AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<int> FlushImageCount()
        {
            return await _dbContext.Image.CountAsync();
        }

        public async Task FlushImages(IEnumerable<Image> imagePathAll)
        {
            var query = _dbContext.Image.FromSqlRaw("Truncate table Image");

            await _dbContext.Image.AddRangeAsync(imagePathAll);

            await _dbContext.SaveChangesAsync();
        }
    }
}
