using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace SimplyPianoStats.Models
{
	public static class Database
	{
		private static string hiddenConnection = "";
		public static string ConnectionString { 
			get {
				hiddenConnection = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
				return hiddenConnection;
			}
		}


		#region Select Methods

		/// <summary>
		/// Select the user from the database with the given ID.
		/// If an error occurrs return null, otherwise, return the results.
		/// </summary>
		/// <param name="id">The ID of the user to search for</param>
		/// <returns>The user's data if that user exists</returns>
		public static DataTable SelectUser(string id)
		{
			DataTable dt = new DataTable();
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM users WHERE id = @id;", con)) {
						da.SelectCommand.Parameters.AddWithValue("@id", id);
						da.Fill(dt);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				dt = null;
			}
			return dt;
		}

		/// <summary>
		/// Select the courses that have been saved in the database and order them by the value they have been set.
		/// </summary>
		/// <returns>A datatable with the courses saved</returns>
		public static DataTable SelectCourses()
		{
			DataTable dt = new DataTable();
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM courses ORDER BY ordering, id;", con)) {
						da.Fill(dt);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				dt = null;
			}
			return dt;
		}

		/// <summary>
		/// Select the songs that have been saved in the database and order them by the value they have been set.
		/// </summary>
		/// <returns>A datatable with the songs saved</returns>
		public static DataTable SelectSongs()
		{
			DataTable dt = new DataTable();
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT a.*, b.name AS course_name, c.date FROM songs AS a LEFT JOIN courses AS b ON a.course_id = b.id LEFT JOIN practice_entries AS c ON a.id = c.song_id ORDER BY a.course_id, a.ordering, a.id;", con)) {
						da.Fill(dt);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				dt = null;
			}
			return dt;
		}

		#endregion

		#region Insert Methods

		/// <summary>
		/// Insert a new course into the database after checking whether it already exists.
		/// If the course name is already taken, alert the user and disregard the following code.
		/// </summary>
		/// <param name="name">The name for the new course being created</param>
		/// <returns>The course object for the course that was just created</returns>
		public static Course InsertCourse(string name)
		{
			// Check whether a course with the given name already exists.
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT EXISTS(SELECT * FROM courses WHERE name = @name);", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@name", name);
						if (cmd.ExecuteScalar().ToString() == "1") {
							throw new Exception("A course with this name already exists.");
						}

						// The course doesn't already exist so insert a new one
						cmd.CommandText = "INSERT INTO courses (name) VALUES (@name);";
						cmd.ExecuteNonQuery();

						// Get the last ID that was saved and return the course object
						cmd.CommandText = "SELECT MAX(id) FROM courses;";
						int latestId = int.Parse(cmd.ExecuteScalar().ToString());
						return new Course() {
							Id = latestId,
							Name = name,
							Ordering = 0
						};
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to add course at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Insert a new song into the database after checking it doesn't already exist.
		/// Check whether a song doesn't already exist with the given course, name and author and alert the user if so.
		/// If the song doesn't already exist, save it to the database.
		/// </summary>
		/// <param name="courseId">The ID for the course that this song is attached to</param>
		/// <param name="name">The name for the new song</param>
		/// <param name="author">The author for the new song</param>
		/// <returns>The newly created Song object</returns>
		public static Song InsertSong(int courseId, string name, string author)
		{
			try {
				// Check whether the provided course exists in the database
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT name FROM courses WHERE id = @id;", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@id", courseId);
						string courseName = cmd.ExecuteScalar()?.ToString();
						if (courseName == null) {
							throw new Exception("The selected course does not exist.");
						}

						// Check whether a song with the given course, name and author already exists.
						cmd.CommandText = "SELECT EXISTS(SELECT * FROM songs WHERE course_id = @id AND name = @name AND author = @author);";
						cmd.Parameters.AddWithValue("@name", name);
						cmd.Parameters.AddWithValue("@author", author);
						if (cmd.ExecuteScalar().ToString() == "1") {
							throw new Exception("A song already exists with the selected course, name and author.");
						}

						// The course doesn't already exist so insert a new one
						cmd.CommandText = "INSERT INTO songs (course_id, name, author) VALUES (@id, @name, @author);";
						cmd.ExecuteNonQuery();

						// Get the last ID that was saved and return the course object
						cmd.CommandText = "SELECT MAX(id) FROM songs;";
						int latestId = int.Parse(cmd.ExecuteScalar().ToString());
						return new Song() {
							Id = latestId,
							CourseId = courseId,
							CourseName = courseName,
							Name = name,
							Author = author,
							Ordering = 0
						};
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to add song at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Update an existing course in the database after checking whether the name already exists.
		/// If the course name is already taken, alert the user and disregard the following code.
		/// If the name is not taken. update the provided course.
		/// </summary>
		/// <param name="name">The name for the course being updated</param>
		/// <returns>The course object for the course that was just updated</returns>
		public static Course UpdateCourse(int id, string name)
		{
			// Check whether a course with the given name already exists that isn't the one provided.
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT EXISTS(SELECT * FROM courses WHERE name = @name AND id != @id);", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@name", name);
						cmd.Parameters.AddWithValue("@id", id);
						if (cmd.ExecuteScalar().ToString() == "1") {
							throw new Exception("A course with this name already exists.");
						}

						// The course doesn't already exist so update the course
						cmd.CommandText = "UPDATE courses SET name = @name WHERE id = @id;";
						if (cmd.ExecuteNonQuery() == 1) {
							return new Course() {
								Id = id,
								Name = name,
								Ordering = 0
							};
						} else {
							throw new Exception("No course has been found with the given ID.");
						}
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to update course at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Update a song in the database after checking the new values don't already exist.
		/// Check whether a song doesn't already exist with the given course, name and author and alert the user if so.
		/// If the song doesn't already exist, update the existing song to the new values.
		/// </summary>
		/// <param name="id">The ID for the song that is being updated</param>
		/// <param name="courseId">The new ID for the course that this song is attached to</param>
		/// <param name="name">The new name for the song</param>
		/// <param name="author">The new author for the song</param>
		/// <returns>The object for the song that has just been updated</returns>
		public static Song UpdateSong(int id, int courseId, string name, string author)
		{
			try {
				// Check whether the provided course exists in the database
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("SELECT name FROM courses WHERE id = @course_id;", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@course_id", courseId);
						string courseName = cmd.ExecuteScalar()?.ToString();
						if (courseName == null) {
							throw new Exception("The selected course does not exist.");
						}

						// Check whether a song with the given course, name and author already exists.
						cmd.CommandText = "SELECT EXISTS(SELECT * FROM songs WHERE course_id = @course_id AND name = @name AND author = @author AND id != @id);";
						cmd.Parameters.AddWithValue("@name", name);
						cmd.Parameters.AddWithValue("@author", author);
						cmd.Parameters.AddWithValue("@id", id);
						if (cmd.ExecuteScalar().ToString() == "1") {
							throw new Exception("A song already exists with the selected course, name and author.");
						}

						// The course doesn't already exist so update the current song
						cmd.CommandText = "UPDATE songs SET course_id = @course_id, name = @name, author = @author WHERE id = @id;";
						if (cmd.ExecuteNonQuery() == 1) {
							return new Song() {
								Id = id,
								CourseId = courseId,
								CourseName = courseName,
								Name = name,
								Author = author,
								Ordering = 0
							};
						} else {
							throw new Exception("No song has been found with the given ID.");
						}
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to edit song at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		#endregion

		#region Delete Methods

		/// <summary>
		/// Delete the course with the given ID.
		/// Check whether the course was deleted and return an error if nothing was deleted
		/// </summary>
		/// <param name="id">The ID of the course to delete</param>
		public static void DeleteCourse(int id)
		{
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("DELETE FROM courses WHERE id = @id;", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@id", id);
						if (cmd.ExecuteNonQuery() == 0) {
							throw new Exception("Course could not be deleted. No course found with the given ID.");
						}
						cmd.CommandText = "UPDATE songs SET course_id = -1 WHERE course_id = @id;";
						cmd.ExecuteNonQuery();
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to delete course at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Delete the song with the given ID.
		/// Check whether the song was deleted and return an error if nothing was deleted
		/// </summary>
		/// <param name="id">The ID of the song to delete</param>
		public static void DeleteSong(int id)
		{
			try {
				using (MySqlConnection con = new MySqlConnection(ConnectionString)) {
					using (MySqlCommand cmd = new MySqlCommand("DELETE FROM songs WHERE id = @id;", con)) {
						con.Open();
						cmd.Parameters.AddWithValue("@id", id);
						if (cmd.ExecuteNonQuery() == 0) {
							throw new Exception("Song could not be deleted. No song found with the given ID.");
						}
					}
				}
			} catch (MySqlException ex) {
				Console.WriteLine(ex.Message);
				throw new Exception("Unable to delete song at this time.");
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Check the given object and convert it into a string. If the object is a DBNull, convert it to an empty string
		/// </summary>
		/// <param name="data">The data to convert to string and check against a DBNull</param>
		/// <returns>The converted data</returns>
		public static string NullCheck(object data)
		{
			return data == DBNull.Value ? string.Empty : data.ToString();
		}

		#endregion
	}
}