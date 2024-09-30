/***************************************************************
 * File Name: CategoryModel.cs
 * Description: Represents the data model for product categories in 
 *              the system. This model is used to store and manage 
 *              category information in the MongoDB database.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: Category names must be unique. The `IsActive` field indicates 
 *        if the category is currently active and can be assigned to 
 *        new products.
 ***************************************************************/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    public class CategoryModel
    {
        // MongoDB document ID, represented as an ObjectId.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Name of the category.
        public string CategoryName { get; set; }

        //Product count
        public int? CategoryCount { get; set; }

        // Status flag indicating if the category is active.
        public bool IsActive { get; set; } = true;
    }
}
