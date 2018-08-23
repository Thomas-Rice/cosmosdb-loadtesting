using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace loadtesting.Controllers
{
    using System.Threading.Tasks;
    using Services;
    using Microsoft.AspNetCore.Mvc;

    public class LoadController : Controller
    {
        private const string RequestChargeTextFile = @"C:\Users\thomas.rice\Documents\LoadTestResults\RequestCharge.txt";
        private const string ResponseTimeTextFile = @"C:\Users\thomas.rice\Documents\LoadTestResults\ResponseTime.txt";
        private const string OutputCsv = @"C:\Users\thomas.rice\Documents\LoadTestResults\RequestChargeCSV.csv";
        private readonly IDocumentDbService _documentDbService;
        private readonly DocumentClient _client;

        public LoadController(IDocumentDbService documentDbService, ITestDocumentService testDocumentService)
        {
            _documentDbService = documentDbService;
            _client = _documentDbService.ReturnClient();

        }

        [ActionName("Write")]
        public ActionResult Create()
        {
            var responseTimes = System.IO.File.ReadAllLines(ResponseTimeTextFile).FirstOrDefault().Split(',').ToList();
            var requestCharges = System.IO.File.ReadAllLines(RequestChargeTextFile).FirstOrDefault().Split(',').ToList();

            using (TextWriter sw = new StreamWriter(OutputCsv))
            {
                for (var i = 0; i < requestCharges.Count; i++)
                {
                    sw.WriteLine("{0}, {1}", responseTimes[i], requestCharges[i]);
                }
            }
            return Ok();
        }


        [ActionName("Read")]
        public async Task<string> Read()
        {
            var query = "SELECT * FROM c WHERE c.availableFrom <= 1534946233 AND c.availableTo >= 1534946233 AND" +
                        " c.warehouse = 'FC01' AND c.serviceLevel = 'Standard Delivery' " +
                        "AND (c.deliveryZone = 'GBR' OR c.deliveryZone = 'GBR\\England' OR c.deliveryZone = 'GBR\\England\\London')";

            var link = UriFactory.CreateDocumentCollectionUri("ReadTest", "Data").ToString();
            var test = _client.CreateDocumentQuery<CalendarEntry>(link, query, new FeedOptions
            {
                PopulateQueryMetrics = true,
            }).AsDocumentQuery();

            var result = await test.ExecuteNextAsync();
            var metrics = result.QueryMetrics;
            var requestCharge = result.RequestCharge;
            var totalMilliseconds = metrics.FirstOrDefault().Value.TotalTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            System.IO.File.AppendAllText(ResponseTimeTextFile, totalMilliseconds + ",");
            System.IO.File.AppendAllText(RequestChargeTextFile, requestCharge + ",");

            var jsonResponseForDisplay = JsonConvert.SerializeObject(test, Formatting.Indented);
            return jsonResponseForDisplay;
        }


    }
}