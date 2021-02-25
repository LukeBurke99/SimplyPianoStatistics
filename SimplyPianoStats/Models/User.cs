using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SimplyPianoStats.Models
{
	public class User
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		[JsonProperty("start_date")]
		public DateTime StartDate { get; set; }

		[JsonProperty("settings")]
		public UserSettings Settings { get; set; }


		/// <summary>
		/// Create a new instance of the user from a DataTable row.
		/// </summary>
		/// <param name="userDetails">The row containing all of the user's details</param>
		public User(DataRow userDetails)
		{
			this.Id = Database.NullCheck(userDetails["id"]);
			this.FirstName = Database.NullCheck(userDetails["first_name"]);
			this.LastName = Database.NullCheck(userDetails["last_name"]);
			string date = Database.NullCheck(userDetails["start_date"]);
			this.StartDate = date == string.Empty ? new DateTime(1900, 1, 1) : DateTime.Parse(date);
			string settings = Database.NullCheck(userDetails["settings"]);
			this.Settings = settings != string.Empty 
				? JsonConvert.DeserializeObject<UserSettings>(settings) 
				: new UserSettings();
		}
	}
}