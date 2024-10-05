/***************************************************************
 * File Name: MongoDBSettings.cs
 * Description: This class holds configuration settings for connecting
 *              to a MongoDB database. The settings include the 
 *              connection string and the database name.
 * Date Created: September 29, 2024
 * Notes: These settings will be injected into the application's 
 *        services to enable interaction with MongoDB.
 ***************************************************************/

namespace EADProject.Models
{
    public class MongoDBSettings
    {
        // MongoDB connection string required to establish a database connection
        public required string ConnectionString { get; set; }

        // Name of the MongoDB database to be used
        public required string DatabaseName { get; set; }
    }
}
