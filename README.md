# Simply Piano Statistics

This is a C# Web API application to store statistics for a Simply Piano app user.

Simply Piano is a mobile application that teaches you to play the piano.  There are hundreds of songs and course lessons to go through and the app will provide you with feedback after playing each one.  One downside to the app is that it doesn't save statistic history so once you click away from the feedback page you will never see those stats again.

This Web API is designed to store and retrieve those statistics after a user has entered them into an HTML web page.

## Installation

Clone the repository and follow the instructions below to get the database connection set up.

1. Create the MySQL database in a location of your choice using the MySQL code inside of the `DatabaseCreation.txt` file.
2. Create a config file in the root of the project folder called `ConnectionStrings.config`.
3. Delete the existing code from the `ConnectionStrings.config` file and enter the following code.

```xml
<connectionStrings>
    <add name="dbConnection" 
         providerName="MySql.Data.MySqlClient" 
         connectionString="YOUR-CONNECTION-STRING-GOES-HERE" />
</connectionStrings>
```
4. Replace the `YOUR-CONNECTION-STRING-GOES-HERE` with your own connection string to the database you have just created.


## Usage

You are free to clone and use this app as you want, but it is recommended that the Web API is used with a user-interface.

You can create an HTML page and use JavaScript or jQuery to interact with the API. See an example jQuery request below.

```javascript
$.ajax({
    url: "https://localhost:44367/data",
    type: "GET",
    dataType: "json",
    beforeSend: function (request) {
        request.setRequestHeader("x-user-token", USER_ID_GOES_HERE);
    },
    success: function (result) {
        console.log(result);
    },
    error: function (a, b, c) {
        console.log(a);
    }
});
```
