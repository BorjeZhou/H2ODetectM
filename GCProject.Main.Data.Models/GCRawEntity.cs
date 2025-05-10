using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCProject.Main.Data.Models;

public class GCRawEntity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long GCRawEntityID { get; set; }

	public long GCSummaryEntityID { get; set; }

	[ForeignKey("GCSummaryEntityID")]
	public virtual GCSummaryEntity GCSummaryEntity { get; set; }

	public ushort Index { get; set; }

	public float H20 { get; set; }

	public float CPS { get; set; }

	public float Width { get; set; }

	public float? Height { get; set; }
}
