using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WdtAsrA1.DAL
{
    public interface IDalDbProxy
    {
        Task<dynamic> ExecuteScalarAsync(string procedure,
            Dictionary<string, dynamic> connParams = null);

        DataTable GetDataTable(string procedure,
            Dictionary<string, dynamic> connParams = null);
        
        void ExecuteNonQuery(string procedure,
            Dictionary<string, dynamic> connParams);
        
    }
}