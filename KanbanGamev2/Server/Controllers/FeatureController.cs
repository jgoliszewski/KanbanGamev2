using Microsoft.AspNetCore.Mvc;
using KanbanGame.Shared;
using KanbanGamev2.Server.Services;

namespace KanbanGamev2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IFeatureService _featureService;

    public FeatureController(IFeatureService featureService)
    {
        _featureService = featureService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Feature>>> GetFeatures()
    {
        var features = await _featureService.GetFeaturesAsync();
        return Ok(features);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Feature>> GetFeature(Guid id)
    {
        var feature = await _featureService.GetFeatureAsync(id);
        if (feature == null)
            return NotFound();
        return Ok(feature);
    }

    [HttpGet("column/{columnId}")]
    public async Task<ActionResult<List<Feature>>> GetFeaturesByColumn(string columnId)
    {
        var features = await _featureService.GetFeaturesByColumnAsync(columnId);
        return Ok(features);
    }

    [HttpPost]
    public async Task<ActionResult<Feature>> CreateFeature(Feature feature)
    {
        var created = await _featureService.CreateFeatureAsync(feature);
        return CreatedAtAction(nameof(GetFeature), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Feature>> UpdateFeature(Guid id, Feature feature)
    {
        if (id != feature.Id)
            return BadRequest();

        try
        {
            var updated = await _featureService.UpdateFeatureAsync(feature);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFeature(Guid id)
    {
        var deleted = await _featureService.DeleteFeatureAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
} 