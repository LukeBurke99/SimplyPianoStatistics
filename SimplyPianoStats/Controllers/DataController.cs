using SimplyPianoStats.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimplyPianoStats.Controllers
{
    public class DataController : MyApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
		{
			try {
                // Check that the user is authorised to use the app and retrieve their data.
                User user = Authorised(Request.Headers);
                DataTable dtCourses = Database.SelectCourses();
                DataTable dtSongs = Database.SelectSongs();

                // Get all of the data together ready to send back to the user.
                AllData allData = new AllData() {
                    User = user,
                    Courses = ConvertTableToList<Course>(dtCourses),
                    Songs = ConvertTableToList<Song>(dtSongs)
                };
                return Request.CreateResponse(HttpStatusCode.OK, allData);
			} catch (Exception ex) {
                // Return the error message as an object that can be handled.
                ErrorMessage error = new ErrorMessage(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
