using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SimplyPianoStats.Models
{
	public class Song
	{
		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonProperty("course_id")]
		public int? CourseId { get; set; }

		[JsonProperty("course_name")]
		public string CourseName { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("ordering")]
		public int? Ordering { get; set; }

		[JsonProperty("last_entry")]
		public DateTime? LastEntry { get; set; }

		public Song() {}


		/// <summary>
		/// Initialie a song object using the DataTable row's data.
		/// </summary>
		/// <param name="songData">The DataTable row to use to initialize an object</param>
		public Song(DataRow songData)
		{
			this.Id = int.Parse(Database.NullCheck(songData["id"]));
			string course = Database.NullCheck(songData["course_id"]);
			this.CourseId = course == "" ? null : (int?)int.Parse(course);
			course = Database.NullCheck(songData["course_name"]);
			this.CourseName = course == "" ? null : course;
			this.Name = Database.NullCheck(songData["name"]);
			this.Author = Database.NullCheck(songData["author"]);
			this.Ordering = int.Parse(Database.NullCheck(songData["ordering"]));
			string d = Database.NullCheck(songData["date"]);
			this.LastEntry = d == "" ? null : (DateTime?)DateTime.Parse(d);
		}
	}
}