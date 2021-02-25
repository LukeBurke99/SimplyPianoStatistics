using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplyPianoStats.Models
{
	public class AllData
	{
		[JsonProperty("user")]
		public User User { get; set; }

		[JsonProperty("courses")]
		public List<Course> Courses { get; set; }

		[JsonProperty("songs")]
		public List<Song> Songs { get; set; }
	}
}