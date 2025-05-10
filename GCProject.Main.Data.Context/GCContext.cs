using System;
using System.IO;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GCProject.Main.Data.Context;

public class GCContext : DbContext
{
	public static string ConnStr { get; private set; }

	public DbSet<GCSummaryEntity> GCSummaryEntities { get; set; }

	public DbSet<GCRawEntity> GCRawEntities { get; set; }

	public static void Init(string file, bool InitDefault = false)
	{
		string path = Path.GetDirectoryName(file);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		GCContext context = new GCContext();
		ConnStr = "Data Source=" + file;
		GCLogger.log.Info("DB Conn " + ConnStr);
		if (InitDefault)
		{
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}
		else
		{
			context.Database.EnsureCreated();
		}
		int year = ConfigModel.Instance.DataRetentionYear;
		ClearRawData(context, 365 * year);
		ClearSummaryData(context, 365 * year);
		UpdateCheck(context);
	}

	public static void ClearRawData(GCContext context, int day)
	{
		DateTime ClearDate = DateTime.Today.AddDays(-day);
		string sql = $"delete from GCRawEntities where GCSummaryEntityID \r\n                            in (select GCSummaryEntityID from GCSummaryEntities where StartDT < '{ClearDate:yyyy-MM-dd}' and IsSampleData = 0)";
		context.Database.ExecuteSqlRaw(sql);
	}

	public static void ClearSummaryData(GCContext context, int day)
	{
		DateTime ClearDate = DateTime.Today.AddDays(-day);
		string sql = $"delete from GCSummaryEntities where StartDT < '{ClearDate:yyyy-MM-dd}'";
		context.Database.ExecuteSqlRaw(sql);
	}

	public static void UpdateCheck(GCContext context)
	{
		update_V1_2_17(context);
		update_V1_2_40(context);
	}

	private static void update_V1_2_17(GCContext context)
	{
		try
		{
			string sql = "ALTER TABLE GCSummaryEntities ADD column \"RealZeroRate\" REAL NULL ";
			context.Database.ExecuteSqlRaw(sql);
			GCLogger.log.Info("update_V1_2_17 脚本更新完成");
		}
		catch
		{
		}
	}

	private static void update_V1_2_40(GCContext context)
	{
		string sql = null;
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"PackageNo\" VARCHAR(100) NULL ";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"Weight\" REAL NULL ";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"Height\" REAL NULL ";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"Diff_H20_OnlyCPS\" REAL NOT NULL DEFAULT 0 ";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"H20_OnlyCPS\" REAL NOT NULL DEFAULT 0";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCSummaryEntities ADD column \"IsTempSelected\" INTEGER NOT NULL DEFAULT 0";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "CREATE INDEX \"IX_GCSummaryEntities_GCRecipeID\" ON \"GCSummaryEntities\" (\"GCRecipeID\");";
			context.Database.ExecuteSqlRaw(sql);
		}
		catch
		{
		}
		try
		{
			sql = "ALTER TABLE GCRawEntities ADD column \"Height\" REAL NULL";
			context.Database.ExecuteSqlRaw(sql);
			GCLogger.log.Info("update_V1_2_40 脚本更新完成");
		}
		catch
		{
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<GCSummaryEntity>(delegate
		{
		});
		modelBuilder.Entity<GCRawEntity>();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseSqlite(ConnStr, delegate(SqliteDbContextOptionsBuilder option)
		{
			option.MaxBatchSize(1000);
		});
	}

	protected void InitDefaultData()
	{
		try
		{
			SaveChanges();
		}
		catch (Exception)
		{
			throw;
		}
	}
}
