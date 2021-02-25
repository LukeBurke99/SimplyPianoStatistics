using SimplyPianoStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimplyPianoStats.Controllers
{
    public class SongsController : MyApiController
    {
		[HttpGet]
		public HttpResponseMessage Get()
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				return Request.CreateResponse(HttpStatusCode.OK, "songs");
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

		/// <summary>
		/// Create a new song with the provided data.
		/// Check if the data is valid and then insert the song into the database, IF the course and name are not already in use.
		/// </summary>
		/// <param name="song">The new song to add to the database</param>
		/// <returns>The song that was just created</returns>
		[HttpPost]
		[Route("songs")]
		public HttpResponseMessage Post([FromBody] Song song)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Check if the data provided is valid
				if (song.Id == null) {
					if (song.CourseId != null) {
						song.Name = song.Name.Trim();
						song.Author = song.Author.Trim();
						if (song.Name.Length > 0 && song.Name.Length <= 255 && song.Author.Length > 0 && song.Author.Length <= 255) {
							song = Database.InsertSong((int)song.CourseId, song.Name, song.Author);
							return Request.CreateResponse(HttpStatusCode.OK, song);
						} else {
							throw new Exception("Please provide a valid name and author (0-255 characters).");
						}
					} else {
						throw new Exception("Please select a course to add this song to.");
					}
				} else {
					throw new Exception("Unable to add song as an ID was provided.");
				}
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

		/// <summary>
		/// Update an existing song with the provided data.
		/// Firstly, check the data is valid and the song that intends to be updated is provided.
		/// Check whether the course, name and author aren't already in use and prevent it from updating if so.
		/// If everything is fine, update the song.
		/// </summary>
		/// <param name="song">The data to use to update the song</param>
		/// <returns>The newly updated song</returns>
		[HttpPut]
		[Route("songs")]
		public HttpResponseMessage Put([FromBody] Song song)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Check if the data provided is valid
				if (song.Id != null) {
					if (song.CourseId != null) {
						song.Name = song.Name.Trim();
						song.Author = song.Author.Trim();
						if (song.Name.Length > 0 && song.Name.Length <= 255 && song.Author.Length > 0 && song.Author.Length <= 255) {
							song = Database.UpdateSong((int)song.Id, (int)song.CourseId, song.Name, song.Author);
							return Request.CreateResponse(HttpStatusCode.OK, song);
						} else {
							throw new Exception("Please provide a valid name and author (0-255 characters).");
						}
					} else {
						throw new Exception("Please select a course to add this song to.");
					}
				} else {
					throw new Exception("Unable to edit song. No ID was provided.");
				}
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

		/// <summary>
		/// Delete the song with the given ID if it exists.
		/// </summary>
		/// <param name="id">The ID of the song that is being deleted</param>
		/// <returns>TRUE if the song was deleted successfully</returns>
		[HttpDelete]
		[Route("songs/{id}")]
		public HttpResponseMessage Delete([FromUri] int id)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Attempt to delete the song with the given ID
				Database.DeleteSong(id);
				// Return true if the song was deleted without fail
				return Request.CreateResponse(HttpStatusCode.OK, true);
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}
	}
}
