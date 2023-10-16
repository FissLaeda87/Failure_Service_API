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
            Assert.Equal("Hello World!", responseContent);
        }
        [Fact]
        public async Task Test_Events()
        {           
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7281/events");            
            
            var response = await client.SendAsync(request);
            EventsDb context = new();
            List<Event> response2 = context.Events.ToList();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var Content = await response.Content.ReadAsStringAsync();
            List<string> responseContent = new List<string>(Content.Split(','));
            Assert.Equal(response2, responseContent);
        }
    }
}