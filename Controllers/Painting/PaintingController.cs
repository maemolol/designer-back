using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
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
	private IMongoCollection<BsonDocument> _mDb;

	public PaintingController(AppDbContext dbContext, MongoService mongo, IMongoDatabase mDb)
	{
		_context = dbContext;
		_mongo = mongo;
		_mDb = mDb.GetCollection<BsonDocument>("images");
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
			query = query.Where(p => p.Id == paintingId);
		if (!string.IsNullOrWhiteSpace(name))
			query = query.Where(p => p.Name != null && p.Name.ToLower().Contains(name.ToLower()));

		var unfilteredPaintings = await query
			.OrderByDescending(p => p.Id)
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
			.FirstOrDefaultAsync(p => p.Id == id);

		if (picture == null)
			return NotFound("Painting not found.");

		return Ok(picture);
	}

	[HttpPost("add")]
	public async Task<IActionResult> CreatePainting([FromBody] CreatePaintingDto dto)
	{
		var strategy = _context.Database.CreateExecutionStrategy();

		return await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// Create Postgres entity
				var painting = new Paintings
				{
					Id = Guid.NewGuid(),
					Heightid = dto.HeightId,
					Widthid = dto.WidthId,
					Categoryid = dto.CategoryId,
					Name = dto.Name,
					Imagelink = dto.ImageLink,
					Price = dto.Price,
					Sold = false
				};

				await _context.Paintings.AddAsync(painting);
				await _context.SaveChangesAsync();

				// Insert into MongoDB

				var mongoDoc = new BsonDocument
				{
					{ "ImageId", painting.Id.ToString() }, // link both DBs
					{ "FilePath", dto.FilePath ?? "" }
				};

				await _mDb.InsertOneAsync(mongoDoc);

				// Commit SQL transaction
				await transaction.CommitAsync();

				return Ok(new
				{
					message = "Painting created",
					id = painting.Id
				});
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();

				return StatusCode(500, $"Error: {ex.Message}");
			}
		});
	}

	[HttpPatch("{id}/edit")]
	public async Task<IActionResult> Patch(Guid id, [FromBody] Paintings patch)
	{
		var painting = await _context.Paintings.FirstOrDefaultAsync(p => p.Id == id);
		if (painting == null)
			return NotFound("Painting not found.");

		if (!string.IsNullOrWhiteSpace(patch.Name)) painting.Name = patch.Name;
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

		var painting = await _context.Paintings.FirstOrDefaultAsync(p => p.Id == id);
		if (painting == null)
			return NotFound("Painting not found.");
		
		using var transaction = await _context.Database.BeginTransactionAsync();
		_context.Paintings.Remove(painting);
		await _context.SaveChangesAsync();
		await transaction.CommitAsync();

		return Ok("Painting deleted.");
	}
}