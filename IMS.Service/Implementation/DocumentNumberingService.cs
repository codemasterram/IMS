using IMS.Data;
using IMS.Data.Infrastructure;
using IMS.Logic.Contract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Implementation
{
    public class DocumentNumberingService : IDocumentNumberingService
    {
        private IRepository<DocumentNumbering> documentNumberingRepository;
        public DocumentNumberingService(
            IRepository<DocumentNumbering> documentNumberingRepo
            )
        {
            this.documentNumberingRepository = documentNumberingRepo;
        }

        public string GetDocumentNo(NTAEnum.eDocumentSetup documentSetup)
        {
            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("@DocumentSetupId", (int)documentSetup);
            var newDocumentNo = documentNumberingRepository.Context.SqlQuery<string>("Select dbo.fn_GetDocumentNo(@documentSetupId, 'N')", p);
            return newDocumentNo.FirstOrDefault();
        }
    }
}
