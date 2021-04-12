using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.Controllers;
using ReservationService.DBContexts;
using ReservationService.Models;
using ReservationService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http.Testing;
using Newtonsoft.Json;

namespace ReservationService.Tests.UnitTests
{
    public class ReservationTests
    {
        private ReservationController _controller;
        private ReservationServiceDatabaseContext _context;

        public ReservationTests()
        {
            var options = new DbContextOptionsBuilder<ReservationServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryReservationDb").Options;

            _context = new ReservationServiceDatabaseContext(options);
            _controller = new ReservationController(_context);
        }
        [Fact]
        private async void ReserveProducts_ShouldReturnStartDateBeforeCurrentDateError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 3, 18), EndDate = new DateTime(2021, 6, 23) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Equal(2, errorList.Count());
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal(pm1, errorList[1].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_INVALID_STARTDATE", errorList[0].Value);
                Assert.Equal("PRODUCT.RESERVE.RESERVATION_TIME_TO_LONG", errorList[1].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnEndDateBeforeCurrentDateError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 6, 18), EndDate = new DateTime(2021, 3, 23) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Equal(2, errorList.Count());
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal(pm1, errorList[1].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_INVALID_ENDDATE", errorList[0].Value);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_ENDDATE_BEFORE_STARTDATE", errorList[1].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnEndDateBeforeStartDateError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 6, 18), EndDate = new DateTime(2021, 6, 10) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_ENDDATE_BEFORE_STARTDATE", errorList[0].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnStartDayWeekendError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 6, 12), EndDate = new DateTime(2021, 6, 17) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.STARTDATE_IN_WEEKEND", errorList[0].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnEndDayWeekendError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 6, 15), EndDate = new DateTime(2021, 6, 19) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.ENDDATE_IN_WEEKEND", errorList[0].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnReservationTimeToLongError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 7, 15), EndDate = new DateTime(2021, 7, 23) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.RESERVATION_TIME_TO_LONG", errorList[0].Value);
            }
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnReservationPeriodAlreadyTakenError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2022, 6, 15), EndDate = new DateTime(2022, 6, 17) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            Reservation reservation = new Reservation() { StartDate = new DateTime(2022, 6, 14), EndDate = new DateTime(2022, 6, 19) };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_ALREADY_RESERVED_IN_PERIOD", errorList[0].Value);
            }
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }

        [Fact]
        private async void ReserveProducts_ShouldReturnNoProductFoundError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 7, 15), EndDate = new DateTime(2021, 7, 16) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.UNAVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Single(errorList);
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[0].Value);
            }
        }
        [Fact]
        private async void ReserveProducts_ShouldReturnNoProductFoundMultipleProductsError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 7, 15), EndDate = new DateTime(2021, 7, 16) };
            ProductModel pm2 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 7, 8), EndDate = new DateTime(2021, 7, 9) };
            model.ProductModels.Add(pm1);
            model.ProductModels.Add(pm2);
            Product product = new Product() { Id = 1, ProductState = ProductState.UNAVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                var errorList = getErrorList(result);
                Assert.Equal(2, errorList.Count());
                Assert.Equal(pm1, errorList[0].Key);
                Assert.Equal(pm2, errorList[1].Key);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[0].Value);
                Assert.Equal("PRODUCT.RESERVE.PRODUCT_NOT_AVAILABLE", errorList[1].Value);
            }
        }


        [Fact]
        private async void ReserveProducts_ShouldReturnSuccessfullReservationError()
        {
            ReserveProductModel model = new ReserveProductModel() { ProductModels = new List<ProductModel>() };
            ProductModel pm1 = new ProductModel { Id = 6, StartDate = new DateTime(2021, 6, 18), EndDate = new DateTime(2021, 6, 23) };
            model.ProductModels.Add(pm1);
            Product product = new Product() { Id = 1, ProductState = ProductState.AVAILABLE, RequiresApproval = true };
            string serializedObject = JsonConvert.SerializeObject(product);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.ReserveProducts(model);
                Assert.IsType<OkResult>(result);
            }
            Assert.Equal(1, _context.Reservations.Count());
        }
        /// <summary>
        /// Gets error from a badrequest error for the Reserve Products tests
        /// </summary>
        /// <param name="result">The IActionResult from the controller call</param>
        /// <returns></returns>
        private List<KeyValuePair<ProductModel, string>> getErrorList(IActionResult result)
        {
            ObjectResult objectResult = Assert.IsType<BadRequestObjectResult>(result);

            var errorIEnumerable = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<ProductModel, string>>>(objectResult.Value);
            var errorList = errorIEnumerable.ToList();
            return errorList;
        } 
    }
}
