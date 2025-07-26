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
    public ActionResult<List<Feature>> GetFeatures()
    {
        var features = _featureService.GetFeatures();
        return Ok(features);
    }

    [HttpGet("{id}")]
    public ActionResult<Feature> GetFeature(Guid id)
    {
        var feature = _featureService.GetFeature(id);
        if (feature == null)
            return NotFound();
        return Ok(feature);
    }

    [HttpGet("column/{columnId}")]
    public ActionResult<List<Feature>> GetFeaturesByColumn(string columnId)
    {
        var features = _featureService.GetFeaturesByColumn(columnId);
        return Ok(features);
    }

    [HttpPost]
    public ActionResult<Feature> CreateFeature(Feature feature)
    {
        var created = _featureService.CreateFeature(feature);
        return CreatedAtAction(nameof(GetFeature), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Feature> UpdateFeature(Guid id, Feature feature)
    {
        if (id != feature.Id)
            return BadRequest();

        try
        {
            var updated = _featureService.UpdateFeature(feature);
            return Ok(updated);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteFeature(Guid id)
    {
        var deleted = _featureService.DeleteFeature(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
} 