
using Failure_Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Options;

namespace MinimalApi.Tests
{
    public class MinimalApiTests
    {
        [Fact]
        public async Task Test_HelloWorld()
        {            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7281/");
            
            var response = await client.SendAsync(request);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal("Welcome to API for checking the health of your web services!\nPlease enter in the address bar: \".../swagger\" to check the service", responseContent);
        }
        [Fact]
        public async Task Test_Events()
        {           
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7281/events");            
            
            var response = await client.SendAsync(request);
            
            var context = new EventsDb();            
            var events = await context.Events.ToListAsync();
            string eventsString = JsonConvert.SerializeObject(events);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(eventsString, responseContent);
        }
    }
}