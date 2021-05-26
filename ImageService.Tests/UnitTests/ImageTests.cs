using ImageService.Controllers;
using ImageService.DBContexts;
using ImageService.Models;
using ImageService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageService.Tests.UnitTests
{
    public class ImageTests
    {
        private readonly ImageController _controller;
        private readonly ImageServiceDatabaseContext _context;

        public ImageTests()
        {
            var options = new DbContextOptionsBuilder<ImageServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryImageDb").Options;

            _context = new ImageServiceDatabaseContext(options);
            _controller = new ImageController(_context);
            SeedImageInMemoryDatabaseWithData(_context);
        }

        [Fact]
        public async Task GetImages_WhenCalled_ReturnListOfImages()
        {
            var result = await _controller.GetImages();

            var objectresult = Assert.IsType<OkObjectResult>(result.Result);
            var images = Assert.IsAssignableFrom<IEnumerable<Image>>(objectresult.Value);

            Assert.Equal(3, images.Count());
            Assert.Equal("BBB", Encoding.ASCII.GetString(images.ElementAt(0).Blob));
            Assert.Equal("ZZZ", Encoding.ASCII.GetString(images.ElementAt(1).Blob));
            Assert.Equal("AAA", Encoding.ASCII.GetString(images.ElementAt(2).Blob));
        }

        [Fact]
        public async Task GetFirstImageByProductId_ReturnImage()
        {
            var result = await _controller.GetFirstImageByProductId(1);

            var objectresult = Assert.IsType<OkObjectResult>(result);
            var images = Assert.IsAssignableFrom<ImageBlobModel>(objectresult.Value);
        }

        [Fact]
        public async Task GetImagesByProductId_ReturnListOfImages()
        {
            var result = await _controller.GetImagesByProductId(1);

            var objectresult = Assert.IsType<OkObjectResult>(result);
            var images = Assert.IsAssignableFrom<IEnumerable<ImageBlobModel>>(objectresult.Value);
            Assert.Equal(3, images.Count());
        }


        private static void SeedImageInMemoryDatabaseWithData(ImageServiceDatabaseContext context)
        {
            var data = new List<Image>
                {
                    new Image { Blob = Encoding.ASCII.GetBytes("BBB"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT},
                    new Image { Blob = Encoding.ASCII.GetBytes("ZZZ"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT },
                    new Image { Blob = Encoding.ASCII.GetBytes("AAA"), LinkedKey = 1, LinkedTableType = LinkedTableType.PRODUCT }
                };
            context.Images.AddRange(data);
            context.SaveChanges();

        }
    }
}
