using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[Route("paintings")]
public class PaintingController : ControllerBase
{
	private readonly AppDbContext database;

	public PaintingController(AppDbContext dbContext)
	{
		database = dbContext;
	}

	private Guid? GetUserId()
	{
		string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return Guid.TryParse(userIdString, out var id) ? id : null;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] int? paintingId, [FromQuery] string? name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
	{
		if (page <= 0) page = 1;
		if (pageSize <= 0 || pageSize > 100) pageSize = 10;

		var query = database.paintings
			.Include(p => p.sizes)
			.Include(p => p.categories)
			.AsQueryable();

		if (paintingId != null)
			query = query.Where(p => p.painting_id == paintingId);
		if (!string.IsNullOrWhiteSpace(name))
			query = query.Where(p => p.name != null && p.name.ToLower().Contains(name.ToLower()));

		int totalCount = await query.CountAsync();
		int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var paintings = await query
			.OrderByDescending(p => p.pictureId)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return Ok(new { currentPage = page, pageSize, totalCount, totalPages, paintings });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var picture = await database.paintings
			.Include(p => p.sizes)
			.Include(p => p.categories)
			.FirstOrDefaultAsync(p => p.id == id);

		if (picture == null)
			return NotFound("Painting not found.");

		return Ok(picture);
	}

	[Authorize(Roles = "site_owner")]
	[HttpPatch("{id}")]
	public async Task<IActionResult> Patch(Guid id, [FromBody] Picture patch)
	{
		var userId = GetUserId();
		if (userId == null)
			return Unauthorized();

		var painting = await database.painting.FirstOrDefaultAsync(p => p.id == id);
		if (painting == null)
			return NotFound("Painting not found.");

		if (!string.IsNullOrWhiteSpace(patch.name)) painting.name = patch.name;
		if (!string.IsNullOrWhiteSpace(patch.size)) painting.description = patch.description;
		if (!string.IsNullOrWhiteSpace(patch.image)) painting.image = patch.image;
		if (!string.IsNullOrWhiteSpace(patch.price)) painting.price = patch.price;
		if (!string.IsNullOrWhiteSpace(patch.external_url)) painting.external_url = patch.external_url;
		if (patch.caegory_id != 0) painting.category_id = patch.category_id;

		using var transaction = await database.Database.BeginTransactionAsync();
		await database.SaveChangesAsync();
		await transaction.CommitAsync();

		return Ok(painting);
	}

	[Authorize(Roles = "site_owner")]
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var userId = GetUserId();
		if (userId == null)
			return Unauthorized();

		var painting = await database.paintings.FirstOrDefaultAsync(p => p.id == id);
		if (painting == null)
			return NotFound("Painting not found.");
		
		using var transaction = await database.Database.BeginTransactionAsync();
		database.paintings.Remove(pet);
		await database.SaveChangesAsync();
		await transaction.CommitAsync();

		return Ok("Painting deleted.");
	}
}