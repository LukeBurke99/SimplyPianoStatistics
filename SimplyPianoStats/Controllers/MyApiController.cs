using SimplyPianoStats.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SimplyPianoStats.Controllers
{
	public class MyApiController : ApiController
	{
		/// <summary>
		/// Check if the user accessing the api is currently authorised to do so.
		/// Do this by checking if they have provided their user ID in the x-user-token header.
		/// If so, check whether that ID is a valid ID stored for use against this API.
		/// </summary>
		/// <param name="headers">The headers sent to the request</param>
		/// <returns>True if the user is authorised, false if they are not</returns>
		protected User Authorised(HttpHeaders headers)
		{
			// Check if the user's ID has been specified and it is a calid user
			bool idPresent = headers.Contains("x-user-token");
			if (idPresent) {
				User user = RetrieveUser(headers.GetValues("x-user-token").First());
				return user;
			}
			throw new Exception("Unable to authorise your request. Please provide your access token.");
		}

		/// <summary>
		/// Check the user's ID against the system and check whether they are authorised to use this API.
		/// </summary>
		/// <param name="id">The ID of the user</param>
		/// <returns>True if the user is authorised, false if they are not</returns>
		private User RetrieveUser(string id)
		{
			DataTable userDetails = Database.SelectUser(id);
			if (userDetails != null) {
				if (userDetails.Rows.Count == 1) {
					User user = new User(userDetails.Rows[0]);
					return user;
				}
                throw new Exception("Couldn't find user. Please provided the CORRECT access token.");
			}
			throw new Exception("Error connecting to server. Please try again.");
		}

		/// <summary>
		/// Take a DataTable and convert it into a list of objects.
		/// Pass in the Type that you want the list to be and create an object for each row, adding that object to the list each time.
		/// </summary>
		/// <typeparam name="T">The Type of the class that you want a list of</typeparam>
		/// <param name="dt">The DataTable with the object data in</param>
		/// <returns>A list of objects</returns>
		protected List<T> ConvertTableToList<T>(DataTable dt)
		{
			List<T> list = new List<T>();

			// Loop over the datatable and create an instance for each row, adding each object to the list.
			foreach (DataRow row in dt.Rows) {
				T obj = (T)Activator.CreateInstance(typeof(T), row);
				list.Add(obj);
			}
			return list;
		}
	}
}