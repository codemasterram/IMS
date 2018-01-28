using IMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.Contract
{
    public interface IDocumentNumberingService
    {
        string GetDocumentNo(NTAEnum.eDocumentSetup documentSetup);
    }
}
