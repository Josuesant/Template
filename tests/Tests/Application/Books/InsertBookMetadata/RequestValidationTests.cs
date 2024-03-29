﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Books.InsertBookMetadata;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using MongoDB.Driver.Linq;
using Tests.TestsInfrasctructure;
using Xunit;

namespace Tests.Application.Books.InsertBookMetadata
{
    public class RequestValidationTests : IntegrationTest
    {
        public RequestValidationTests()
        {
            RebuildDatabase();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_validate_required_fields(string noValue)
        {
            var addBookRequest = new InsertBookMetadataRequest
            {
                Name = noValue,
                Description = noValue,
                Publisher = noValue,
                Author = noValue,
                Origin = new InsertBookMetadataRequest.AuthorLocation
                {
                    Planet = noValue,
                    System = noValue
                },
                GalacticYear = 10_001
            };

            Func<Task> insertBookMetadata = async () =>
                await Handle<InsertBookMetadataRequest, InsertBookMetadataResponse>(addBookRequest);

            insertBookMetadata.Should().ThrowAsync<ValidationException>().Result.Which
                .Errors.Should().HaveCount(6);
        }

        [Fact]
        public void Should_validate_that_origin_exists()
        {
            var request = new InsertBookMetadataRequest
            {
                Name = "Fictional Book Name",
                Description =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent justo nulla, pellentesque lacinia enim et, dictum finibus augue. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Morbi fringilla vestibulum ipsum. Nam auctor maximus magna, ac posuere odio dignissim at. Cras et pharetra nibh. Donec laoreet pellentesque finibus. Maecenas dictum elit vel eros semper pharetra. Sed commodo imperdiet dolor vitae fringilla. Aenean sit amet fermentum sem, id posuere sem. Phasellus tempus urna quis vulputate semper. Donec vestibulum sem ipsum, eget tincidunt mauris malesuada eget. Duis lacus nisl, facilisis ac erat vel, euismod tempus sapien. Pellentesque vitae sodales nisi. Sed feugiat justo tincidunt vehicula accumsan. Vestibulum vestibulum fringilla libero, id venenatis nibh venenatis ac. Pellentesque faucibus ut ex porttitor interdum.",
                Publisher = "Solar System Publishing Inc.",
                Author = "Evangeline Mustache",
                Origin = null,
                GalacticYear = 10_001
            };

            Func<Task> insertBookMetadata = async () =>
                await Handle<InsertBookMetadataRequest, InsertBookMetadataResponse>(request);

            _ = insertBookMetadata.Should().ThrowExactlyAsync<ValidationException>().Result.Which
                .Errors.Should().HaveCount(1)
                .And
                .Subject.Where(a => a.ErrorMessage == "Origin is required");
        }
    }
}