using Failure_Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;





var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ���������� POST ������� ��� ������� SLA
app.MapPost("/calculate_sla", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();

    // ��������� ������ �� POST-�������
    var requestBody = await context.Request.ReadFromJsonAsync<SLARequest>();
    var serviceType = requestBody.Type;
    var date = DateTime.Parse(requestBody.Date);
    var serviceName = requestBody.ServiceName;

    // ���������� ������ API ��� ��������� ������ � ����������������� �������
    var apiResponse = await httpClient.GetAsync($"https://localhost:7281/services/{serviceName}/availability?date={date}");

    if (apiResponse.IsSuccessStatusCode)
    {
        var apiContent = await apiResponse.Content.ReadAsStringAsync();
        var availabilityData = JsonSerializer.Deserialize<AvailabilityData>(apiContent);

        // ������ ������� ������� � �������� �� ������ ������� � ������� ����/���/������/����
        // � ����������� �� ���� ������� (hour/day/month/year)
        var downtime = CalcualteDowntime(availabilityData, serviceType);
        var percentage = CalculatePercentage(availabilityData, serviceType);

        // ����������� ���������� �������� � ������� JSON
        var response = new SLAResponse
        {
            Downtime = downtime,
            Percentage = percentage
        };
        await context.Response.WriteAsJsonAsync(response);
    }
    else
    {
        // � ������ ������ API-�������, ������� ��������������� HTTP-������ ���
        context.Response.StatusCode = (int)apiResponse.StatusCode;
    }
});

// ���������� POST ������� ��� ��������� ������� ����������
app.MapPost("/summary_stats", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();

    // ��������� ������ �� POST-�������
    var requestBody = await context.Request.ReadFromJsonAsync<SummaryStats>();
    var startDate = DateTime.Parse(requestBody.StartDate);
    var endDate = DateTime.Parse(requestBody.EndDate);

    // ���������� ������ API ��� ��������� ������� ���������� ������� ������� ���� �������� �� ��������� ������
    var apiResponse = await httpClient.GetAsync($"https://localhost:7281/stats/summary?start={startDate}&end={endDate}");

    if (apiResponse.IsSuccessStatusCode)
    {
        var apiContent = await apiResponse.Content.ReadAsStringAsync();
        var summaryStats = JsonSerializer.Deserialize<SummaryStatsRequest>(apiContent);

        // ����������� ���������� ������� ���������� � ������� JSON
        var response = new SummaryStatsResponse
        {
            SummaryStats = summaryStats
        };
        await context.Response.WriteAsJsonAsync(response);
    }
    else
    {
        // � ������ ������ API-�������, ������� ��������������� HTTP-������ ���
        context.Response.StatusCode = (int)apiResponse.StatusCode;
    }
});



app.Run();

async Task RegisterEventAsync(string name, string status, string message)
{
    // �������� ������� � ����������� � �������
    var ev = new Events
    {
        name = name,
        status = status,
        message = message,
        timestamp = DateTime.Now
    };

    // ������������ ������� � JSON-������
    var requestBody = JsonConvert.SerializeObject(ev);

    // ����������� JSON-������ � ������� � ��������� ���� �����������
    var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

    // �������� POST-������� �� ��������� URL
    using (var httpClient = new HttpClient())
    {
        var requestUrl = "https://localhost:7281/register";
        var response = await httpClient.PostAsync(requestUrl, requestContent);

        if (response.IsSuccessStatusCode)
        {
            // ��������� ��������� ������ �������
            var createdEvent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("������� ������� ����������������:");
            Console.WriteLine(createdEvent);
        }
        else
        {
            // ��������� ���������� ������ �������
            Console.WriteLine($"������ ��� ����������� �������. ��� ������: {response.StatusCode}");
        }
    }
}

class SLARequest
{
    public string Type { get; set; }
    public string Date { get; set; }
    public string ServiceName { get; set; }
}

class SLAResponse
{
    public TimeSpan Downtime { get; set; }
    public double Percentage { get; set; }
}

class SummaryStatsRequest
{
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}

class SummaryStatsResponse
{
    public IDictionary<string, TimeSpan> SummaryStats { get; set; }
}

class ServicePayload
{
    public string Name { get; set; }
    public string Status { get; set; }
}


