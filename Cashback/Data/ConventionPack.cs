using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Cashback.Data
{
    public class ConventionPack
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public class IdGeneratorConvention : ConventionBase, IPostProcessingConvention
        {
            public void PostProcess(BsonClassMap classMap)
            {
                var idMemberMap = classMap.IdMemberMap;
                if (idMemberMap != null && idMemberMap.MemberName == "Id" && idMemberMap.MemberType == typeof(ObjectId))
                    idMemberMap.SetIdGenerator(ObjectIdGenerator.Instance);

                if (idMemberMap != null && idMemberMap.MemberName == "Id" && idMemberMap.MemberType == typeof(string))
                    idMemberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
            }
        }
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void UseConventionMongo()
        {
            var conventionPack = new MongoDB.Bson.Serialization.Conventions.ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("Pack", conventionPack, x => true);
        }
    }
}