using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SimplyPianoStats.Models
{
	public class PracticeEntry
	{
		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonProperty("user_id")]
		public string UserId { get; set; }

		[JsonProperty("song_id")]
		public int? SongId { get; set; }

		[JsonProperty("training")]
		public char? Training { get; set; }

		[JsonProperty("notes")]
		public int? Notes { get; set; }

		[JsonProperty("notes_hit")]
		public int? NotesHit { get; set; }

		[JsonProperty("timing")]
		public int? Timing { get; set; }

		[JsonProperty("assistance")]
		public int? Assistance { get; set; }

		[JsonProperty("stars")]
		public char? Stars { get; set; }

		[JsonProperty("date")]
		public DateTime? Date { get; set; }

		public PracticeEntry() {}


		public PracticeEntry(DataRow entryData)
		{
			this.Id = int.Parse(Database.NullCheck(entryData["id"]));
			this.UserId = Database.NullCheck(entryData["user_id"]);
			this.SongId = int.Parse(Database.NullCheck(entryData["song_id"]));
			this.Training = Database.NullCheck(entryData["training"]).ToCharArray()[0];
			this.Notes = int.Parse(Database.NullCheck(entryData["notes"]));
			this.NotesHit = int.Parse(Database.NullCheck(entryData["notes_hit"]));
			this.Timing = int.Parse(Database.NullCheck(entryData["timing"]));
			this.Assistance = int.Parse(Database.NullCheck(entryData["assistance"]));
			this.Stars = Database.NullCheck(entryData["stars"]).ToCharArray()[0];
			this.Date = DateTime.Parse(Database.NullCheck(entryData["date"]));
		}
	}
}