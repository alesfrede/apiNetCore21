﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace ApiXUnitTestProject
{
    /// <inheritdoc />
    /// <summary>
    /// https ://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1 viewFallbackFrom=aspnetcore-2.0
    /// </summary>
    public partial class Integration : IClassFixture<CustomWebApplicationFactory<Api213.Startup>>
    {

        private readonly CustomWebApplicationFactory<Api213.Startup> _factory;
        private HttpClient _client;

        public Integration(CustomWebApplicationFactory<Api213.Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false

            });
        }

        [Theory]
        [InlineData("/")]

        public async Task Get_EndpointsReturnSuccessAndCorrectContentTypeSwagger(string url)
        {
            // Arrange
            _client = _factory.CreateClient(); 

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html", response.Content.Headers.ContentType.ToString());
        }


        [Theory]
        [InlineData("/api/v2/pets")]
        [InlineData("/api/v2/pets/apetsample")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentTypeJson(string url)
        {
            // Arrange


            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/v2/pets?sort=-id%2C-name&fields=name%2Cid&page=1&count=10")]
        [InlineData("/api/v2/pets?embed=Owner&sort=-id%2C-name&fields=name%2Cid&page=1&count=10")]
        [InlineData("/api/v2/pets?embed=Owner")]
        public async Task GetSortedandPagedSuccess(string url)
        {
            //_client = _factory.CreateClient(); 

            var newIdeas = Utilities.GetTestEntitiesPet();
            // Act 
            var response = await _client.GetAsync(url);
            // Assert 
            response.EnsureSuccessStatusCode();

            var listDtos = JsonConvert.DeserializeObject<IEnumerable<PetTestDto>>(await response.Content.ReadAsStringAsync());
            //Assert.Equal(listDtos.Count(), newIdeas.Count());
            Assert.Equal(1, listDtos.Count(x => x.Name == newIdeas.First().Name));
        }

        [Theory]
        [InlineData("/api/v1/pets")]
        [InlineData("/api/v2/pets?embed=failhere&sort=-failhere%2C-name&fields=failhere%2Cid&page=1&count=10")]
        [InlineData("/api/v2/pets?page=failhere&count=10")]
        [InlineData("/api/v2/pets?embed=failhere")]
        public async Task GetAllFailandReportDetails(string url)
        {
         
            // Act 
            var response = await _client.GetAsync(url);
            // Assert 

            var json = await response.Content.ReadAsStringAsync();
            ((int)response.StatusCode).ShouldBeInRange(202, 511);

            var errorDetails = JsonConvert.DeserializeObject<ErrorDetailsTests>(json);
            errorDetails.Status.ShouldNotBeNull();
            errorDetails.Title.Length.ShouldBeInRange(1, 150);
            errorDetails.Detail.Length.ShouldNotBeNull();
            errorDetails.Errors.Count.ShouldBeInRange(0, 10);
            
            errorDetails.HttpMethod.Length.ShouldBeInRange(3, 10);
            errorDetails.Type.ShouldContain("http");
            errorDetails.Instance.ShouldContain("/");


        }


        [Theory]
        [InlineData("/api/v2/pets/Bebe")]

        public async Task GetOneSuccess(string url)
        {
            // Arrange
            //  var newIdea = new PetInput { Id = 1, Name = "name", Description = "desc" };
            //  await _client.PostAsJsonAsync("/api/v2/pets", newIdea);
            var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == "Bebe");

            // Act
            var response = await _client.GetAsync(url);
             
            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.Name); 
        }


        [Theory]
        [InlineData("/api/v2/pets/Beb2e")]
        public async Task GetOneNotFoundFail(string url)
        {
            // Arrange
  
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    
        }


        //[Theory]
        //[InlineData("/api/v2/pets/'''")]
        //public async Task GetOneNotBadRequestFail(string url)
        //{
        //    // Arrange 

        //    // Act
        //    var response = await _client.GetAsync(url);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        //}




        [Theory]
        [InlineData("/api/v2/pets/Search?namelike=")]
        [InlineData("/api/v2/pets/Search?embed=Owner&namelike=")]
        public async Task SearchSuccess(string url)
        {
            // Arrange
 
            var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == "Bebe");

            // Act
            var response = await _client.GetAsync(url+newIdea.Name);

            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<IEnumerable<PetTestDto>>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.First().Name);
        }

        [Theory]
        [InlineData("/api/v2/pets/Search?namelike=Bebe&embed=Owner")]
        public async Task SearchAndIncludePropertiesSuccess(string url)
        {
            // Arrange

            var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == "Bebe");

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var strjson = await response.Content.ReadAsStringAsync();
            var ideareturnDto = JsonConvert.DeserializeObject<IEnumerable<PetTestDto>>(strjson);
            var petTestDtos = ideareturnDto as PetTestDto[] ?? ideareturnDto.ToArray();
            Assert.Equal(newIdea.Name, petTestDtos.First().Name);
            Assert.NotNull(petTestDtos.First().Owner);
        }

        [Theory]
        [InlineData("/api/v2/pets/Search?namelike=")]
        public async Task SearchFailNotfound(string url)
        {
            // Arrange

           
            // Act
            var response = await _client.GetAsync(url + "xxxx");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreatePostReturnsBadRequestForMissingNameValue()
        {
            // Arrange
            var newIdea = new PetTestDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/v2/pets", newIdea);


            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var errorDescription = await response.Content.ReadAsStringAsync();
            errorDescription.ShouldContain("field is required");


        }

        [Fact]
        public async Task CreatePostReturnsCreatedAndOkModel()
        {
            // Arrange
            var newIdea = new PetTestDto(1000, "name1000", "desc1000");

            // Act
            var response = await _client.PostAsJsonAsync("/api/v2/pets", newIdea);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.Name);

        }

        [Fact]
        public async Task ReplacePutReturnsOkModel()
        {
            // Arrange
            var newIdea = new PetTestDto(2000, "name2000", "desc2000");
            await _client.PostAsJsonAsync("/api/v2/pets", newIdea);
            var updateDto = new PetTestDto(2001, "name2000", "descupdate");
            // Act
            var response = await _client.PutAsJsonAsync("/api/v2/pets", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(updateDto.Name, ideareturnDto.Name);

        }

        [Fact]
        public async Task ReplacePutReturnFailNotFound()
        {
            // Arrange

            var updateDto = new PetTestDto(2001, "xxxx", "descupdate");
            // Act
            var response = await _client.PutAsJsonAsync("/api/v2/pets", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreatePost_ErrorDuplicate()
        {
            // Arrange 
            var newIdea = new PetTestDto(100, "name2", "desc2");
            await _client.PostAsJsonAsync("/api/v2/pets", newIdea);

            // Act
            var response = await _client.PostAsJsonAsync("/api/v2/pets", newIdea);

            // Assert
            Assert.NotEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            //await response.Content.ReadAsStringAsync();
            //Assert.Equal(newIdea.Name, ideareturnDto.Name);

        }


        [Theory]
        [InlineData("/api/v2/pets/", "Bebe")]
        [InlineData("/api/v2/pets/", "Dogui")]
        public async Task PatchOneSuccess(string url, string id)
        {
            var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == id);
            var patch = new JArray(new JObject(
                new JProperty("op", "replace"),
                new JProperty("path", "description"),
                new JProperty("value", "Do"))).ToString();
            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.Name);
            Assert.Equal("Do", ideareturnDto.Description);
        }


        [Theory]
        [InlineData("/api/v2/pets/Merge/", "Bebe")]
        [InlineData("/api/v2/pets/Merge/", "Dogui")]
        public async Task PatchMergeOneSuccess(string url, string id)
        {
            var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == id);
            var patch = new JObject(new JProperty("Description", "Do")).ToString();

            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.Name);
            Assert.Equal("Do", ideareturnDto.Description);
        }


        [Theory]
        [InlineData("/api/v2/pets/", "Bebe")]
        public async Task PatchOneFailJsonPatchPropertyname(string url, string id)
        {
           // var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == id);
            var patch = new JArray(new JObject(
                new JProperty("op", "replace"),
                new JProperty("path", "Propiedad1"),
                new JProperty("value", "Do")
            )).ToString();
            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        }

        [Theory]
        [InlineData("/api/v2/pets/", "Bebe")]
        [InlineData("/api/v2/pets/Merge/", "Bebe")]
        public async Task PatchOneFailJsonPatchFormat(string url, string id)
        {
           var patch = new JArray(new JObject(
                new JProperty("op", ""),
                new JProperty("path", "Propiedad1"),
                new JProperty("value", "Do")
            )).ToString();
            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        }

        [Theory]
        [InlineData("/api/v2/pets/", "Bebe")]
        public async Task PatchOneFailJsonPatchPropertyError(string url, string id)
        {
          //  var newIdea = Utilities.GetTestEntitiesPet().First(e => e.Name == id);
            var patch = new JArray(new JObject(
                new JProperty("op", "replace"),
                new JProperty("path", "id"),
                new JProperty("value", -1)
            )).ToString();
            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        }

        [Theory]
        [InlineData("/api/v2/pets/", "xxxx")]
        [InlineData("/api/v2/pets/Merge/", "xxxx")]
        public async Task PatchOneNotFound(string url, string id)
        {
            var patch = new JArray(new JObject(
                new JProperty("op", "replace"),
                new JProperty("path", "description"),
                new JProperty("value", "Do")
            )).ToString();
            var body = new StringContent(patch, Encoding.UTF8, "application/json-patch+json");

            // Act
            var response = await _client.PatchAsync(url + id, body);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }

        [Fact]
        public async Task DeleteReturnsOkModel()
        {
            // Arrange
            var newIdea = new PetTestDto(2000, "name3000", "desc2000");
            await _client.PostAsJsonAsync("/api/v2/pets", newIdea);
            
            // Act
            var response = await _client.DeleteAsync("/api/v2/pets/"+ newIdea.Name);

            // Assert
            response.EnsureSuccessStatusCode();
            var ideareturnDto = JsonConvert.DeserializeObject<PetTestDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal(newIdea.Name, ideareturnDto.Name);

        }

        [Fact]
        public async Task DeleteReturnFailNotFound()
        {
            // Arrange

            // Act
            var response = await _client.DeleteAsync("/api/v2/pets/xxxx");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
} 
