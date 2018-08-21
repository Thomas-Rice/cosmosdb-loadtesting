namespace loadtesting.Controllers
{
    using System.Threading.Tasks;
    using Services;
    using Microsoft.AspNetCore.Mvc;

    public class LoadController : Controller
    {
        private readonly IDocumentDbService _documentDbService;
        private readonly ITestDocumentService _testDocumentService;

        public LoadController(IDocumentDbService documentDbService, ITestDocumentService testDocumentService)
        {
            _documentDbService = documentDbService;
            _testDocumentService = testDocumentService;
        }

        [ActionName("Write")]
        public async Task<ActionResult> CreateAsync()
        {
            await _documentDbService.AddItemAsync(_testDocumentService.GetDocument());
            return Ok();
        }

        [ActionName("Read")]
        public async Task<ActionResult> ReadAsync()
        {
            await _documentDbService.ReadItemAsync(Constants.IdForReadTesting);
            return Ok();
        }
    }
}