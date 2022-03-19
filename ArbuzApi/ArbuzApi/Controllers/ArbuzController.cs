using ArbuzApi.Helpers;
using ArbuzApi.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArbuzApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArbuzController : ControllerBase
    {
        public ArbuzController(ILogger<ArbuzController> logger)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            string productsUrl = "https://arbuz.kz/ru/almaty/catalog/cat/13-produkty#/";
            List<string> categoriesUrl = await GetProductCategoryUrls(productsUrl);
            var globalProductList = new List<ProductItem>();
            foreach (var url in categoriesUrl)
            {
                var products = await GetProductsList(url);
                globalProductList.AddRange(products);
            }

            return Ok(globalProductList);
        }

        private async Task<List<string>> GetProductCategoryUrls(string productsUrl)
        {
            var resultUrls = new List<string>();
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(productsUrl);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            return resultUrls;
        }

        private static async Task<List<ProductItem>> GetProductsList(string arbuzUrl)
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetStringAsync(arbuzUrl);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var items = htmlDoc.DocumentNode.DivsByClass("product-item-body").ToList();
            var products = new List<ProductItem>();
            int id = 0;
            foreach (var item in items)
            {
                id++;
                var product = new ProductItem();
                var productItemText = item.DivsByClass("product-item-text").FirstOrDefault();
                var productItemPrice = item.DivsByClass("product-item-price").FirstOrDefault();
                product.Description = productItemText.Descendants("a").FirstOrDefault().InnerText.ToCleanString();
                var priceParts = productItemPrice
                                    .Descendants("div").FirstOrDefault()
                                    .Descendants("div").FirstOrDefault()
                                    .ChildNodes
                                    .Where(ch => ch.Name == "b" || ch.Name == "small")
                                    .Select(ch => ch.InnerText);
                product.Price = string.Join("", priceParts);
                product.Id = id;

                products.Add(product);
            }

            return products;
        }
    }
}
