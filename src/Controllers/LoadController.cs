using System.Linq;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace loadtesting.Controllers
{
    using System.Threading.Tasks;
    using Services;
    using Microsoft.AspNetCore.Mvc;

    public class LoadController : Controller
    {
        private readonly IDocumentDbService _documentDbService;
        private readonly ITestDocumentService _testDocumentService;
        private DocumentClient _client;

        public LoadController(IDocumentDbService documentDbService, ITestDocumentService testDocumentService)
        {
            _documentDbService = documentDbService;
            _testDocumentService = testDocumentService;
            _client = _documentDbService.ReturnClient();

        }

        [ActionName("Write")]
        public async Task<ActionResult> CreateAsync()
        {
            await _documentDbService.AddItemAsync(_testDocumentService.GetDocument());
            return Ok();
        }

        [ActionName("Read")]
        public string Read()
        {
            var query = "SELECT * FROM c WHERE c.availableFrom <= 1534946233 AND c.availableTo >= 1534946233 AND" +
                        " c.warehouse = 'FC01' AND c.serviceLevel = 'Standard Delivery' " +
                        "AND (c.deliveryZone = 'GBR' OR c.deliveryZone = 'GBR\\England' OR c.deliveryZone = 'GBR\\England\\London')";
            //var query = "SELECT * FROM c WHERE c.id = \"toRead\"";

            var link = UriFactory.CreateDocumentCollectionUri("ReadTest", "Data").ToString();
            var test = _client.CreateDocumentQuery<CalendarEntry>(link, query).AsEnumerable().ToList();
            var test2 = JsonConvert.SerializeObject(test, Formatting.Indented);
            return test2;
        }
    }
}