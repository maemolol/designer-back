using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Controllers;

[ApiController]
[Route("paintings")]
public class PaintingController : ControllerBase
{
	private readonly AppDbContext _context;
	private MongoService _mongo;

	public PaintingController(AppDbContext dbContext, MongoService mongo)
	{
		_context = dbContext;
		_mongo = mongo;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] Guid? paintingId, [FromQuery] string? name, [FromQuery] int page = 1, int category = 1)
	{
		int pageSize = 16;
		if (page <= 0) page = 1;

		var query = _context.Paintings
			.Include(p => p.Height)
			.Include(p => p.Width)
			.Include(p => p.Category)
			.AsQueryable();

		if (paintingId != null)
			query = query.Where(p => p.id == paintingId);
		if (!string.IsNullOrWhiteSpace(name))
			query = query.Where(p => p.name != null && p.name.ToLower().Contains(name.ToLower()));

		var unfilteredPaintings = await query
			.OrderByDescending(p => p.id)
			.ToListAsync();

		var uncountedPaintings = unfilteredPaintings.Where(p => p.Categoryid == category);

        int totalCount = uncountedPaintings.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

		var paintings = uncountedPaintings
			.Skip((page - 1) * pageSize)
			.Take(pageSize);

        try
		{
            return Ok(new { currentPage = page, pageSize, totalCount, totalPages, paintings });
        }
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var picture = await _context.Paintings
			.Include(p => p.Height)
			.Include(p => p.Width)
			.FirstOrDefaultAsync(p => p.id == id);

		if (picture == null)
			return NotFound("Painting not found.");

		return Ok(picture);
	}

	[HttpPost("add")]
	public async Task<IActionResult> CreatePainting([FromBody] PaintingAddDto request)
	{
		Console.WriteLine($"Raw name from the request: {request.name}");
		if(request == null) return BadRequest(new {error = "Body required."});
		if(request.category_id == null) return BadRequest(new {error = "Category ID required."});
		if(string.IsNullOrWhiteSpace(request.image_link)) return BadRequest(new {error = "Image link required."});

		var name = (request.name ?? "untitled").Trim();
		byte[] raw_image = System.IO.File.ReadAllBytes(request.image_path);

		try{
			if(await _context.Paintings.AnyAsync(p => p.name == name))
				return Conflict(new {error = "Painting with this name already exists."});
			var painting = new Paintings
			{
				Heightid = request.height_id,
				Widthid = request.width_id,
				Categoryid = request.category_id,
				name = name,
				Imagelink = request.image_link
			};
			_context.Paintings.Add(painting);
			await _context.SaveChangesAsync();

			_mongo.SaveImageAsync(request.image_link, raw_image);
			
			var message = $"Painting {request.name} was added";
			return Ok(new {message});
		} catch (Exception ex)
		{
			return StatusCode (500, new {error = ex.Message});
		}
	}

	[HttpPatch("{id}/edit")]
	public async Task<IActionResult> Patch(Guid id, [FromBody] Paintings patch)
	{
		var painting = await _context.Paintings.FirstOrDefaultAsync(p => p.id == id);
		if (painting == null)
			return NotFound("Painting not found.");

		if (!string.IsNullOrWhiteSpace(patch.name)) painting.name = patch.name;
		if (!string.IsNullOrWhiteSpace(patch.Imagelink)) painting.Imagelink = patch.Imagelink;
		if (patch.Heightid != null) painting.Heightid = patch.Heightid;
		if (patch.Widthid != null) painting.Widthid = patch.Widthid;
		if (patch.Categoryid != null) painting.Categoryid = patch.Categoryid;

		using var transaction = await _context.Database.BeginTransactionAsync();
		await _context.SaveChangesAsync();
		await transaction.CommitAsync();

		return Ok(painting);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{

		var painting = await _context.Paintings.FirstOrDefaultAsync(p => p.id == id);
		if (painting == null)
			return NotFound("Painting not found.");
		
		using var transaction = await _context.Database.BeginTransactionAsync();
		_context.Paintings.Remove(painting);
		await _context.SaveChangesAsync();
		await transaction.CommitAsync();

		return Ok("Painting deleted.");
	}
}