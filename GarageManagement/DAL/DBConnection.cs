using System.Configuration;

namespace GarageManagement.DAL
{
    public static class DBConnection
    {
        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["GarageDB"].ConnectionString;
    }
}