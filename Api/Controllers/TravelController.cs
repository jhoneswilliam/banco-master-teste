namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TravelController(ILogger<TravelController> logger, ITravelService travelService) : ControllerBase
{
    [HttpGet(Name = "ObterRotaMaisBarata")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<BestCostTravelResponse>> Get([FromQuery] string origem, [FromQuery] string destino, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(origem) || string.IsNullOrEmpty(destino))
        {
            return BadRequest("É necessario infomar a origem e o destino");
        }

        try
        {
            var result = await travelService.GetBestCostTravel(origem, destino, ct);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost(Name = "ImportarArquivo")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ImportTravelCosts([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is missing or empty.");

        if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("File is not a valid CSV.");

        using (var stream = file.OpenReadStream())
        {
            byte[] buffer = new byte[3];
            await stream.ReadAsync(buffer, 0, 3);

            if (!(buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF))
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var travelCosts = new List<ImportTravelsCostRequest>();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var columns = line.Split(',');

                    if (columns.Length != 3 ||
                        !int.TryParse(columns[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var cost))
                    {
                        return BadRequest($"Invalid format in line: {line}");
                    }

                    var travelCost = new ImportTravelsCostRequest
                    {
                        Departure = columns[0],
                        Arrival = columns[1],
                        Cost = cost
                    };

                    travelCosts.Add(travelCost);
                }
            }
            try
            {
                await travelService.ImportCostTravel(travelCosts);
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }


            return Ok("Importado com sucesso!");
        }
    }
}
