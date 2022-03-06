using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using YOLO.CaseStudy.Business.Interfaces;
using YOLO.CaseStudy.Entities;

namespace YOLO.CaseStudy.Business.Implementations
{
    public class CaseStudyBusiness : ICaseStudyBusiness
    {
        private const string Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private readonly IGraphQLClient _graphQlClient;
        private readonly IWebHostEnvironment _environment;

        public CaseStudyBusiness(IGraphQLClient graphQlClient, IWebHostEnvironment environment)
        {
            this._graphQlClient = graphQlClient;
            this._environment = environment;
        }

        public Result ReverseText(WordProcessType processType = WordProcessType.ReverseCharacters)
        {
            string data;

            if (processType == WordProcessType.ReverseWords)
            {
                var words = Text.Split(' ');
                Array.Reverse(words);
                data = string.Join(" ", words.Select(x => x.EndsWith('.') ? $".{x.Trim('.')}" : x));
                return new SuccessResult<string>(data);
            }

            var charArray = Text.ToCharArray();
            Array.Reverse(charArray);
            data = new string(charArray);
            return new SuccessResult<string>(data);
        }

        public async Task<Result> Iterator(CancellationToken cancellationToken)
        {
            async Task<(bool isOk, int output)> Iterate(int number)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                //TODO: might implement some logic that might return false
                return (true, number);
            }

            var tasks = Enumerable.Range(1, 1000).Select(Iterate);
            var result = await Task.WhenAll(tasks);
            var data = result.Where(x => x.isOk).Select(x => x.output);
            return new SuccessResult<IEnumerable<int>>(data);
        }

        public async Task<Result> CalculateChecksum(CancellationToken cancellationToken)
        {
            var path = _environment.IsDevelopment() ? $"{Directory.GetParent(Directory.GetCurrentDirectory())}\\100MB.bin" : $"{AppContext.BaseDirectory}/100MB.bin";

            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(path);
            var hash = await md5.ComputeHashAsync(stream, cancellationToken);
            var data = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            return new SuccessResult<string>(data);
        }

        public async Task<Result> GetAssets(CancellationToken cancellationToken)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                            query AssetsQuery {
                              assets(filter: null, sort: { marketCapRank: ASC }) {
                                assetName
                                assetSymbol
                                marketCap
                              }
                            }",
                OperationName = "AssetsQuery",
                Variables = null
            };

            var assetResponse = await _graphQlClient.SendQueryAsync<AssetResponse>(request, cancellationToken);
            var assetData = assetResponse.Data?.Assets;
            if (assetData == null)
            {
                return new ErrorResult("No assets returned");
            }

            request.OperationName = "PricesQuery";
            const string pricesQuery = @"
                            query PricesQuery {{
                              markets(filter: {{baseSymbol: {{_in: [{0}]}}}}) {{
                                marketSymbol
                                baseSymbol
                                ticker {{
                                   lastPrice
                                }}
                              }}
                            }}";

            for (var i = 1; i <= 100; i++)
            {
                if (i % 20 != 0)
                {
                    continue;
                }

                request.Query = string.Format(pricesQuery, string.Join(",", assetData.Skip(i - 20).Take(20).Select(x => $@"""{x.AssetSymbol}""")));

                var priceResponse = await _graphQlClient.SendQueryAsync<PriceResponse>(request, cancellationToken);
                var priceData = priceResponse.Data?.Markets;
                if (priceData == null)
                {
                    continue;
                }

                var groupedPrices = priceData.GroupBy(x => x.BaseSymbol);

                foreach (var prices in groupedPrices)
                {
                    var asset = assetData.FirstOrDefault(x => string.Equals(x.AssetSymbol, prices.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (asset == null)
                    {
                        continue;
                    }

                    asset.Prices = prices.Where(x => x.Ticker != null).Select(x => new AssetPrice
                    {
                        Price = x.Ticker.LastPrice,
                        MarketSymbol = x.MarketSymbol
                    });
                }
            }


            var result = new SuccessResult<ICollection<Asset>>(assetData);

            return result;
        }
    }
}
