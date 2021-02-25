using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SimplyPianoStats.Models
{
	public class Course
	{
		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("ordering")]
		public int? Ordering { get; set; }
		public Course() {}


		/// <summary>
		/// Initialize a course object using the data from a DataTable row.
		/// </summary>
		/// <param name="courseData">The DataTable row to use to create this object</param>
		public Course(DataRow courseData)
		{
			this.Id = int.Parse(Database.NullCheck(courseData["id"]));
			this.Name = Database.NullCheck(courseData["name"]);
			this.Ordering = int.Parse(Database.NullCheck(courseData["ordering"]));
		}

	}
}