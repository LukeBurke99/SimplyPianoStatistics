using SimplyPianoStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimplyPianoStats.Controllers
{
	public class CoursesController : MyApiController
	{
		[HttpGet]
		public HttpResponseMessage Get()
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				return Request.CreateResponse(HttpStatusCode.OK, "courses");
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.Forbidden, error);
			}
		}

		/// <summary>
		/// Create a new course with the provided data.
		/// check if the data is valid and then insert the course into the database, IF the name is not already in use.
		/// </summary>
		/// <param name="course">The new course to add to the database</param>
		/// <returns>The course that was just created</returns>
		[HttpPost]
		[Route("courses")]
		public HttpResponseMessage Post([FromBody] Course course)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Check if the data provided is valid
				if (course.Id == null) {
					course.Name = course.Name.Trim();
					if (course.Name.Length > 0 && course.Name.Length <= 50) {
						course = Database.InsertCourse(course.Name);
						return Request.CreateResponse(HttpStatusCode.OK, course);
					} else {
						throw new Exception("Please provide a valid name (0-50 characters).");
					}
				} else {
					throw new Exception("Unable to add course as an ID was provided.");
				}
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

		/// <summary>
		/// Update an existing course with the provided data.
		/// Firstly, check the data is valid and the course that intends to be updated is provided.
		/// Check whether the name is already in use and prevent it from updating if so.
		/// If everything is fine, update the course.
		/// </summary>
		/// <param name="course">The data to use to update the course</param>
		/// <returns>The newly updated course</returns>
		[HttpPut]
		[Route("courses")]
		public HttpResponseMessage Put([FromBody] Course course)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Check if the data provided is valid
				if (course.Id != null) {
					course.Name = course.Name.Trim();
					if (course.Name.Length > 0 && course.Name.Length <= 50) {
						course = Database.UpdateCourse((int)course.Id, course.Name);
						return Request.CreateResponse(HttpStatusCode.OK, course);
					} else {
						throw new Exception("Please provide a valid name (0-50 characters).");
					}
				} else {
					throw new Exception("Unable to edit course. No ID found..");
				}
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

		/// <summary>
		/// Delete the course with the given ID if it exists.
		/// </summary>
		/// <param name="id">The ID of the course that is being deleted</param>
		/// <returns>TRUE if the course was deleted successfully</returns>
		[HttpDelete]
		[Route("courses/{id}")]
		public HttpResponseMessage Delete([FromUri] int id)
		{
			try {
				// Check if the user is authorised
				User user = Authorised(Request.Headers);

				// Attempt to delete the course with the given ID
				Database.DeleteCourse(id);
				// Return true if the course was deleted without fail
				return Request.CreateResponse(HttpStatusCode.OK, true);
			} catch (Exception ex) {
				ErrorMessage error = new ErrorMessage(ex.Message);
				return Request.CreateResponse(HttpStatusCode.BadRequest, error);
			}
		}

	}
}
