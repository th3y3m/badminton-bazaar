using BusinessObjects;
using Nest;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;

        public ElasticsearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task<bool> IsAvailableAsync()
        {
            var pingResponse = await _client.PingAsync();
            return pingResponse.IsValid; // Returns true if the ping is successful
        }

        public async Task CreateIndexAsync(string indexName)
        {
            // Check if the index exists
            var existsResponse = await _client.Indices.ExistsAsync(indexName);
            if (existsResponse.Exists)
            {
                Console.WriteLine($"Index {indexName} already exists.");
                return;
            }

            // Define index settings and mappings
            var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            .Phonetic("my_phonetic_filter", ph => ph
                                .Encoder(PhoneticEncoder.DoubleMetaphone)
                                .Replace(false)
                            )
                            .Synonym("synonym_filter_1", sf => sf
                                .Synonyms("laptop, notebook, portable computer")
                            )
                            .WordDelimiterGraph("word_delimiter_custom", wdg => wdg
                                .GenerateWordParts()
                                .GenerateNumberParts()
                                .CatenateWords()
                                .CatenateNumbers()
                                .CatenateAll()
                                .PreserveOriginal()
                            )
                            .NGram("ngram_filter", ng => ng
                                .MinGram(3)
                                .MaxGram(4)
                            )
                            .Stemmer("my_stemmer", st => st
                                .Language("english")
                            )
                            .Shingle("shingle_filter", sh => sh
                                .MinShingleSize(2)
                                .MaxShingleSize(3)
                            )
                        )
                        .Analyzers(an => an
                            .Custom("my_custom_analyzer", ca => ca
                                .Tokenizer("whitespace")
                                .Filters("lowercase", "synonym_filter_1",
                                         "word_delimiter_custom",
                                         "my_phonetic_filter", "ngram_filter", "my_stemmer", "shingle_filter")
                            )
                        )
                    )
                )
                .Map(m => m
                    .Properties(ps => ps
                        .Text(t => t
                            .Name("ProductName")
                            .Analyzer("my_custom_analyzer")
                            .Fields(f => f
                                .Keyword(k => k
                                    .Name("raw")
                                )
                                .Text(ft => ft
                                    .Name("folded")
                                    .Analyzer("my_custom_analyzer")
                                )
                            )
                        )
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Error creating index: {createIndexResponse.ServerError.Error.Reason}");
            }

            Console.WriteLine($"Index {indexName} created successfully.");
        }

        public async Task IndexProductsAsync(List<Product> products)
        {
            foreach (var product in products)
            {
                var indexResponse = await _client.IndexAsync(product, i => i
                    .Index("products")
                    .Id(product.ProductId)
                );

                if (!indexResponse.IsValid)
                {
                    // Log error message if indexing fails
                    Console.WriteLine($"Error indexing product {product.ProductId}: {indexResponse.ServerError.Error.Reason}");
                }
                else
                {
                    Console.WriteLine($"Product {product.ProductId} indexed successfully");
                }
            }
        }

        public async Task<List<Product>> SearchProductsByNameAsync(string name)
        {
            var response = await _client.SearchAsync<Product>(s => s
                .Index("products")
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            sh => sh.Match(m => m
                                .Field(f => f.ProductName)
                                .Query(name)
                                .Fuzziness(Fuzziness.Auto)
                                .Operator(Operator.Or)
                            ),
                            sh => sh.Match(m => m
                                .Field(f => f.ProductName.Suffix("phonetic"))
                                .Query(name)
                                .Fuzziness(Fuzziness.Auto)
                            ),
                            sh => sh.MatchPhrase(mp => mp
                                .Field(f => f.ProductName)
                                .Query(name)
                                .Slop(1)
                            ),
                            sh => sh.Wildcard(wc => wc
                                .Field(f => f.ProductName)
                                .Value($"*{name}*")
                            )
                        )
                        .MinimumShouldMatch(1)
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Search failed: {response.ServerError.Error.Reason}");
            }

            return response.Documents.ToList();
        }
    }
}
